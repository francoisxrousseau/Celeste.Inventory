namespace Celeste.Inventory.Api.Options;

/// <summary>
///     Represents logging-specific observability configuration.
/// </summary>
public sealed class ObservabilityLoggingOptions
{
    /// <summary>
    ///     Gets the configuration section name for logging options.
    /// </summary>
    public const string SectionName = $"{ObservabilityOptions.SectionName}:Logging";

    /// <summary>
    ///     Gets or sets the OTLP endpoint used for log exports.
    /// </summary>
    public string OtlpEndpoint { get; set; } = "http://localhost:4317";

    /// <summary>
    ///     Gets or sets a value indicating whether logging scopes are exported.
    /// </summary>
    public bool IncludeScopes { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether state values are parsed into structured attributes.
    /// </summary>
    public bool ParseStateValues { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether formatted log messages are included alongside structured state.
    /// </summary>
    public bool IncludeFormattedMessage { get; set; }
}
