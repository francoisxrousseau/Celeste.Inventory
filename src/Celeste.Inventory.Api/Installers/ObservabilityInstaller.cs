namespace Celeste.Inventory.Api.Installers;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Celeste.Inventory.Api.Options;
using Emit.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

/// <summary>
///     Registers OpenTelemetry observability services for traces, metrics, and logs.
/// </summary>
public static class ObservabilityInstaller
{
    /// <summary>
    ///     Adds production-style observability services using configuration-driven OpenTelemetry options.
    /// </summary>
    /// <param name="services">
    ///     The service collection being configured.
    /// </param>
    /// <param name="configuration">
    ///     The application configuration root.
    /// </param>
    /// <param name="environment">
    ///     The current host environment.
    /// </param>
    /// <returns>
    ///     The same service collection for chaining.
    /// </returns>
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services
            .AddOptions<ObservabilityOptions>()
            .Bind(configuration.GetSection(ObservabilityOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ServiceName), "Observability service name is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.ServiceNamespace), "Observability service namespace is required.")
            .ValidateOnStart();

        services
            .AddOptions<ObservabilityMetricsOptions>()
            .Bind(configuration.GetSection(ObservabilityMetricsOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.PrometheusScrapingEndpointPath), "Prometheus scraping endpoint path is required.")
            .Validate(options => options.PrometheusScrapingEndpointPath.StartsWith('/'), "Prometheus scraping endpoint path must start with '/'.")
            .ValidateOnStart();

        services
            .AddOptions<ObservabilityTracingOptions>()
            .Bind(configuration.GetSection(ObservabilityTracingOptions.SectionName))
            .Validate(options => Uri.TryCreate(options.OtlpEndpoint, UriKind.Absolute, out _), "Tracing OTLP endpoint must be a valid absolute URI.")
            .ValidateOnStart();

        services
            .AddOptions<ObservabilityLoggingOptions>()
            .Bind(configuration.GetSection(ObservabilityLoggingOptions.SectionName))
            .Validate(options => Uri.TryCreate(options.OtlpEndpoint, UriKind.Absolute, out _), "Logging OTLP endpoint must be a valid absolute URI.")
            .ValidateOnStart();

        var observabilityOptions = configuration.GetSection(ObservabilityOptions.SectionName).Get<ObservabilityOptions>()
            ?? new ObservabilityOptions();
        var metricsOptions = configuration.GetSection(ObservabilityMetricsOptions.SectionName).Get<ObservabilityMetricsOptions>()
            ?? new ObservabilityMetricsOptions();
        var tracingOptions = configuration.GetSection(ObservabilityTracingOptions.SectionName).Get<ObservabilityTracingOptions>()
            ?? new ObservabilityTracingOptions();
        var loggingOptions = configuration.GetSection(ObservabilityLoggingOptions.SectionName).Get<ObservabilityLoggingOptions>()
            ?? new ObservabilityLoggingOptions();
        var serviceVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";
        var deploymentEnvironment = string.IsNullOrWhiteSpace(observabilityOptions.DeploymentEnvironment)
            ? environment.EnvironmentName
            : observabilityOptions.DeploymentEnvironment;
        var ignoredRequestPaths = CreateIgnoredRequestPaths(metricsOptions.PrometheusScrapingEndpointPath);

        services.AddOpenTelemetry()
            .ConfigureResource(resource => ConfigureResource(resource, observabilityOptions, deploymentEnvironment, serviceVersion))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.Filter = httpContext => ShouldTraceRequest(httpContext, ignoredRequestPaths);
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })
                    .AddEmitInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(tracingOptions.OtlpEndpoint);
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter("System.Net.Http")
                    .AddEmitInstrumentation(options =>
                    {
                        options.EnrichWithTag("service.name", observabilityOptions.ServiceName);
                        options.EnrichWithTag("deployment.environment", deploymentEnvironment);
                    })
                    .AddPrometheusExporter();
            });

        services.AddHttpClient();

        services.AddLogging(logging =>
        {
            logging.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(CreateResourceBuilder(observabilityOptions, deploymentEnvironment, serviceVersion));
                options.IncludeScopes = loggingOptions.IncludeScopes;
                options.ParseStateValues = loggingOptions.ParseStateValues;
                options.IncludeFormattedMessage = loggingOptions.IncludeFormattedMessage;
                options.AddOtlpExporter(exporterOptions =>
                {
                    exporterOptions.Endpoint = new Uri(loggingOptions.OtlpEndpoint);
                    exporterOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                });
            });
        });

        return services;
    }

    /// <summary>
    ///     Maps the Prometheus scraping endpoint for the configured metrics path.
    /// </summary>
    /// <param name="app">
    ///     The web application being configured.
    /// </param>
    /// <returns>
    ///     The same application for chaining.
    /// </returns>
    public static WebApplication UseObservability(this WebApplication app)
    {
        var metricsOptions = app.Services.GetRequiredService<IOptions<ObservabilityMetricsOptions>>().Value;
        app.UseOpenTelemetryPrometheusScrapingEndpoint(metricsOptions.PrometheusScrapingEndpointPath);
        return app;
    }

    /// <summary>
    ///     Creates the OpenTelemetry resource builder used by traces, metrics, and logs.
    /// </summary>
    /// <param name="options">
    ///     The shared observability options.
    /// </param>
    /// <param name="deploymentEnvironment">
    ///     The deployment environment resource attribute value.
    /// </param>
    /// <param name="serviceVersion">
    ///     The service version reported to backends.
    /// </param>
    /// <returns>
    ///     A configured resource builder.
    /// </returns>
    private static ResourceBuilder CreateResourceBuilder(
        ObservabilityOptions options,
        string deploymentEnvironment,
        string serviceVersion)
    {
        return ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: options.ServiceName,
                serviceNamespace: options.ServiceNamespace,
                serviceVersion: serviceVersion)
            .AddAttributes([
                new KeyValuePair<string, object>("deployment.environment", deploymentEnvironment)
            ]);
    }

    /// <summary>
    ///     Applies shared resource metadata to the builder provided by OpenTelemetry hosting extensions.
    /// </summary>
    /// <param name="resourceBuilder">
    ///     The resource builder being configured.
    /// </param>
    /// <param name="options">
    ///     The shared observability options.
    /// </param>
    /// <param name="deploymentEnvironment">
    ///     The deployment environment resource attribute value.
    /// </param>
    /// <param name="serviceVersion">
    ///     The service version reported to backends.
    /// </param>
    private static void ConfigureResource(
        ResourceBuilder resourceBuilder,
        ObservabilityOptions options,
        string deploymentEnvironment,
        string serviceVersion)
    {
        resourceBuilder
            .AddService(
                serviceName: options.ServiceName,
                serviceNamespace: options.ServiceNamespace,
                serviceVersion: serviceVersion)
            .AddAttributes([
                new KeyValuePair<string, object>("deployment.environment", deploymentEnvironment)
            ]);
    }

    /// <summary>
    ///     Determines whether a request should be captured by HTTP server instrumentation.
    /// </summary>
    /// <param name="httpContext">
    ///     The active HTTP context.
    /// </param>
    /// <param name="ignoredRequestPaths">
    ///     Request path prefixes that should be excluded from telemetry.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> when the request should be instrumented; otherwise, <see langword="false"/>.
    /// </returns>
    private static bool ShouldTraceRequest(
        HttpContext httpContext,
        IReadOnlyList<PathString> ignoredRequestPaths)
    {
        return !ignoredRequestPaths.Any(ignoredRequestPath => httpContext.Request.Path.StartsWithSegments(ignoredRequestPath));
    }

    /// <summary>
    ///     Creates the set of low-value request paths that should be excluded from instrumentation.
    /// </summary>
    /// <param name="prometheusScrapingEndpointPath">
    ///     The Prometheus scraping endpoint path configured for the application.
    /// </param>
    /// <returns>
    ///     The path list used by HTTP instrumentation filters.
    /// </returns>
    private static IReadOnlyList<PathString> CreateIgnoredRequestPaths(string prometheusScrapingEndpointPath)
    {
        return
        [
            new PathString(prometheusScrapingEndpointPath),
            new PathString("/health"),
            new PathString("/swagger"),
            new PathString("/openapi"),
            new PathString("/favicon.ico"),
            new PathString("/robots.txt")
        ];
    }
}
