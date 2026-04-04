namespace Celeste.Inventory.Core.Domain;

/// <summary>
///     Represents a manufacturer associated with products in the catalog.
/// </summary>
public sealed class Manufacturer : AuditableEntity
{
    /// <summary>
     ///     Gets the unique identifier of the manufacturer.
     /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets the display name of the manufacturer.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
     ///     Gets the contact email for the manufacturer.
     /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
     ///     Gets the contact phone number for the manufacturer.
     /// </summary>
    public string? ContactPhone { get; set; }
}
