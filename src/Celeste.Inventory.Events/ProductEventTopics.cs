namespace Celeste.Inventory.Events;

/// <summary>
///     Provides Kafka topic names for product events.
/// </summary>
public static class ProductEventTopics
{
    /// <summary>
    ///     The Kafka topic used for product events.
    /// </summary>
    public const string Product = "celeste.inventory.product";
}
