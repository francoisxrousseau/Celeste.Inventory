namespace Celeste.Inventory.Events;

/// <summary>
///     Creates product integration event payloads.
/// </summary>
public static class ProductEventFactory
{
    /// <summary>
    ///     Creates a created-event payload.
    /// </summary>
    public static ProductEvent Created(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        string status,
        string? category,
        IReadOnlyList<string>? tags,
        string? user,
        DateTime date)
        => Create(id, manufacturerId, name, description, status, category, tags, ProductEventTypes.Created, user, date);

    /// <summary>
    ///     Creates an updated-event payload.
    /// </summary>
    public static ProductEvent Updated(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        string status,
        string? category,
        IReadOnlyList<string>? tags,
        string? user,
        DateTime date)
        => Create(id, manufacturerId, name, description, status, category, tags, ProductEventTypes.Updated, user, date);

    /// <summary>
    ///     Creates a deleted-event payload.
    /// </summary>
    public static ProductEvent Deleted(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        string status,
        string? category,
        IReadOnlyList<string>? tags,
        string? user,
        DateTime date)
        => Create(id, manufacturerId, name, description, status, category, tags, ProductEventTypes.Deleted, user, date);

    /// <summary>
    ///     Creates a variant created-event payload.
    /// </summary>
    public static ProductEvent VariantCreated(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        string status,
        string? category,
        IReadOnlyList<string>? tags,
        Variant variant,
        string? user,
        DateTime date)
        => Create(id, manufacturerId, name, description, status, category, tags, ProductEventTypes.VariantCreated, variant, user, date);

    /// <summary>
    ///     Creates a variant updated-event payload.
    /// </summary>
    public static ProductEvent VariantUpdated(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        string status,
        string? category,
        IReadOnlyList<string>? tags,
        Variant variant,
        string? user,
        DateTime date)
        => Create(id, manufacturerId, name, description, status, category, tags, ProductEventTypes.VariantUpdated, variant, user, date);

    /// <summary>
    ///     Creates a variant deleted-event payload.
    /// </summary>
    public static ProductEvent VariantDeleted(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        string status,
        string? category,
        IReadOnlyList<string>? tags,
        Variant variant,
        string? user,
        DateTime date)
        => Create(id, manufacturerId, name, description, status, category, tags, ProductEventTypes.VariantDeleted, variant, user, date);

    private static ProductEvent Create(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        string status,
        string? category,
        IReadOnlyList<string>? tags,
        string eventType,
        string? user,
        DateTime date)
        => Create(id, manufacturerId, name, description, status, category, tags, eventType, null, user, date);

    private static ProductEvent Create(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        string status,
        string? category,
        IReadOnlyList<string>? tags,
        string eventType,
        Variant? variant,
        string? user,
        DateTime date)
    {
        return new ProductEvent
        {
            Id = id,
            EventType = eventType,
            ManufacturerId = manufacturerId,
            ProductDetails = new ProductDetails
            {
                Name = name,
                Description = description,
            },
            Variant = variant,
            Status = status,
            Category = category,
            Tags = tags?.ToList(),
            User = user ?? string.Empty,
            Date = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime(),
        };
    }

}
