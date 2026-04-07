namespace Celeste.Inventory.Infrastructure.Documents;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
///     Represents the MongoDB manufacturer document.
/// </summary>
public sealed class ManufacturerDocument
{
    /// <summary>
    ///     Gets or sets the manufacturer identifier.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the manufacturer name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the normalized manufacturer name used for queries and uniqueness.
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the optional contact email.
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    ///     Gets or sets the optional contact phone.
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    ///     Gets or sets the user that created the record.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    ///     Gets or sets the UTC creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the user that last updated the record.
    /// </summary>
    public string? LastUpdatedBy { get; set; }

    /// <summary>
    ///     Gets or sets the UTC last update timestamp.
    /// </summary>
    public DateTime? LastUpdatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the user that deleted the record.
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     Gets or sets the UTC delete timestamp.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
