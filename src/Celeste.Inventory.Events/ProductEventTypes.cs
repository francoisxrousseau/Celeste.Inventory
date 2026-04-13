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

    /// <summary>
    ///     The event type for variant creation.
    /// </summary>
    public const string VariantCreated = "product.variant.created";

    /// <summary>
    ///     The event type for variant updates.
    /// </summary>
    public const string VariantUpdated = "product.variant.updated";

    /// <summary>
    ///     The event type for variant deletion.
    /// </summary>
    public const string VariantDeleted = "product.variant.deleted";
}
