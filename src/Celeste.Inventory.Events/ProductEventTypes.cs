namespace Celeste.Inventory.Events;

/// <summary>
///     Provides product event type names.
/// </summary>
public static class ProductEventTypes
{
    /// <summary>
    ///     The event type for product creation.
    /// </summary>
    public const string Created = "product.created";

    /// <summary>
    ///     The event type for product updates.
    /// </summary>
    public const string Updated = "product.updated";

    /// <summary>
    ///     The event type for product deletion.
    /// </summary>
    public const string Deleted = "product.deleted";
}
