namespace Celeste.Inventory.Api.Options;

/// <summary>
///     Represents shared OpenTelemetry resource metadata for the inventory API.
/// </summary>
public sealed class ObservabilityOptions
{
    /// <summary>
    ///     Gets the configuration section name for observability options.
    /// </summary>
    public const string SectionName = "Observability";

    /// <summary>
    ///     Gets or sets the service name reported to telemetry backends.
    /// </summary>
    public string ServiceName { get; set; } = "celeste-inventory-api";

    /// <summary>
    ///     Gets or sets the service namespace reported to telemetry backends.
    /// </summary>
    public string ServiceNamespace { get; set; } = "celeste";

    /// <summary>
    ///     Gets or sets the deployment environment resource attribute value.
    /// </summary>
    public string DeploymentEnvironment { get; set; } = "development";
}
