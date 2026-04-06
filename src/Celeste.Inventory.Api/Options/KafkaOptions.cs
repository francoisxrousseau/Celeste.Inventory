namespace Celeste.Inventory.Api.Options;

/// <summary>
///     Represents Kafka and schema registry configuration for the API bootstrap layer.
/// </summary>
public sealed class KafkaOptions
{
    /// <summary>
    ///     Gets the configuration section name for Kafka options.
    /// </summary>
    public const string SectionName = "Kafka";

    /// <summary>
    ///     Gets or sets the Kafka bootstrap servers.
    /// </summary>
    public string BootstrapServers { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the schema registry URL.
    /// </summary>
    public string SchemaRegistryUrl { get; set; } = string.Empty;
}
