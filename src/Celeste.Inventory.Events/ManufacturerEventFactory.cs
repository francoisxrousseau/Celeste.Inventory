namespace Celeste.Inventory.Events;

/// <summary>
///     Creates manufacturer integration event payloads.
/// </summary>
public static class ManufacturerEventFactory
{
    /// <summary>
    ///     Creates a created-event payload.
    /// </summary>
    public static ManufacturerEvent Created(Guid id, string? name, string? contactEmail, string? contactPhone, string? user, DateTime date)
        => Create(id, name, contactEmail, contactPhone, ManufacturerEventTypes.Created, user, date);

    /// <summary>
    ///     Creates an updated-event payload.
    /// </summary>
    public static ManufacturerEvent Updated(Guid id, string? name, string? contactEmail, string? contactPhone, string? user, DateTime date)
        => Create(id, name, contactEmail, contactPhone, ManufacturerEventTypes.Updated, user, date);

    /// <summary>
    ///     Creates a deleted-event payload.
    /// </summary>
    public static ManufacturerEvent Deleted(Guid id, string? name, string? contactEmail, string? contactPhone, string? user, DateTime date)
        => Create(id, name, contactEmail, contactPhone, ManufacturerEventTypes.Deleted, user, date);

    private static ManufacturerEvent Create(
        Guid id,
        string? name,
        string? contactEmail,
        string? contactPhone,
        string eventType,
        string? user,
        DateTime date)
    {
        return new ManufacturerEvent
        {
            Id = id,
            Name = name,
            ContactEmail = contactEmail,
            ContactPhone = contactPhone,
            EventType = eventType,
            User = user ?? string.Empty,
            Date = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime(),
        };
    }
}
