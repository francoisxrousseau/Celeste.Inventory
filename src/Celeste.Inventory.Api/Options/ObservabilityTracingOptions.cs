namespace Celeste.Inventory.Api.Options;

/// <summary>
///     Represents tracing-specific observability configuration.
/// </summary>
public sealed class ObservabilityTracingOptions
{
    /// <summary>
    ///     Gets the configuration section name for tracing options.
    /// </summary>
    public const string SectionName = $"{ObservabilityOptions.SectionName}:Tracing";

    /// <summary>
    ///     Gets or sets the OTLP endpoint used for trace exports.
    /// </summary>
    public string OtlpEndpoint { get; set; } = "http://localhost:4317";
}
