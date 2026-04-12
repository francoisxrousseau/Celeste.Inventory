namespace Celeste.Inventory.Infrastructure.Documents;

using Celeste.Inventory.Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
///     Represents the MongoDB product document.
/// </summary>
public sealed class ProductDocument
{
    /// <summary>
    ///     Gets or sets the product identifier.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the manufacturer identifier.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public Guid ManufacturerId { get; set; }

    /// <summary>
    ///     Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the normalized product name used for queries.
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the optional product description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the product status.
    /// </summary>
    public ProductStatus Status { get; set; }

    /// <summary>
    ///     Gets or sets the product category.
    /// </summary>
    public ProductCategory Category { get; set; }

    /// <summary>
    ///     Gets or sets the optional product tags.
    /// </summary>
    public List<string>? Tags { get; set; }

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
