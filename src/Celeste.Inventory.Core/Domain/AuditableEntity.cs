namespace Celeste.Inventory.Core.Domain;

/// <summary>
///     Represents the shared audit fields tracked for domain entities.
/// </summary>
public abstract class AuditableEntity
{
    /// <summary>
    ///     Gets the user that created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    ///     Gets the UTC timestamp when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Gets the user that last updated the entity.
    /// </summary>
    public string? LastUpdatedBy { get; set; }

    /// <summary>
    ///     Gets the UTC timestamp when the entity was last updated.
    /// </summary>
    public DateTime? LastUpdatedAt { get; set; }

    /// <summary>
    ///     Gets the user that soft deleted the entity.
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     Gets the UTC timestamp when the entity was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    ///     Gets a value indicating whether the entity has been soft deleted.
    /// </summary>
    public bool IsDeleted => DeletedAt.HasValue;
}
