namespace Celeste.Inventory.Api.Options;

/// <summary>
///     Represents metrics-specific observability configuration.
/// </summary>
public sealed class ObservabilityMetricsOptions
{
    /// <summary>
    ///     Gets the configuration section name for metrics options.
    /// </summary>
    public const string SectionName = $"{ObservabilityOptions.SectionName}:Metrics";

    /// <summary>
    ///     Gets or sets the Prometheus scraping endpoint path exposed by the API.
    /// </summary>
    public string PrometheusScrapingEndpointPath { get; set; } = "/metrics";
}
