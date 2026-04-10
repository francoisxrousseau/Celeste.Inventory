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
        IReadOnlyList<string>? tags,
        string? user,
        DateTime date)
        => Create(id, manufacturerId, name, description, status, tags, ProductEventTypes.Created, user, date);

    /// <summary>
    ///     Creates an updated-event payload.
    /// </summary>
    public static ProductEvent Updated(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        string status,
        IReadOnlyList<string>? tags,
        string? user,
        DateTime date)
        => Create(id, manufacturerId, name, description, status, tags, ProductEventTypes.Updated, user, date);

    /// <summary>
    ///     Creates a deleted-event payload.
    /// </summary>
    public static ProductEvent Deleted(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        string status,
        IReadOnlyList<string>? tags,
        string? user,
        DateTime date)
        => Create(id, manufacturerId, name, description, status, tags, ProductEventTypes.Deleted, user, date);

    private static ProductEvent Create(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        string status,
        IReadOnlyList<string>? tags,
        string eventType,
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
            Status = status,
            Tags = tags?.ToList(),
            User = user ?? string.Empty,
            Date = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime(),
        };
    }
}
