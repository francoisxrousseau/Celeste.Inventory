namespace Celeste.Inventory.Api.Installers;

using Emit.Kafka.HealthChecks;
using Emit.MongoDB.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
///     Registers and maps liveness/readiness health checks for the API.
/// </summary>
public static class HealthChecksInstaller
{
    private const string LivenessTag = "live";
    private const string ReadinessTag = "ready";

    /// <summary>
    ///     Adds process liveness and dependency readiness health checks.
    /// </summary>
    /// <param name="services">
    ///     The service collection being configured.
    /// </param>
    /// <returns>
    ///     The same service collection for chaining.
    /// </returns>
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            // Liveness stays process-only and never depends on external systems.
            .AddCheck(
                name: "self",
                check: () => HealthCheckResult.Healthy(),
                tags: [LivenessTag])
            .AddEmitKafka(
                name: "emit-kafka",
                failureStatus: HealthStatus.Degraded,
                tags: [ReadinessTag, "emit", "messaging"],
                timeout: TimeSpan.FromSeconds(5))
            .AddEmitMongoDB(
                name: "emit-mongodb",
                failureStatus: HealthStatus.Unhealthy,
                tags: [ReadinessTag, "emit", "database"],
                timeout: TimeSpan.FromSeconds(5));

        return services;
    }

    /// <summary>
    ///     Maps process liveness and dependency readiness endpoints.
    /// </summary>
    /// <param name="app">
    ///     The web application being configured.
    /// </param>
    /// <returns>
    ///     The same application for chaining.
    /// </returns>
    public static WebApplication MapApiHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks(
            "/health/live",
            new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(LivenessTag)
            });

        app.MapHealthChecks(
            "/health/ready",
            new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(ReadinessTag)
            });

        return app;
    }
}
