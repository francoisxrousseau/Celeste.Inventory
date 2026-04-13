namespace Celeste.Inventory.Infrastructure.Documents;

using Celeste.Inventory.Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
///     Represents an embedded MongoDB variant document.
/// </summary>
public sealed class VariantDocument
{
    /// <summary>
    ///     Gets or sets the variant identifier.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the stock keeping unit.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the variant price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    ///     Gets or sets the optional discount details.
    /// </summary>
    public DiscountInformationsDocument? DiscountInformations { get; set; }

    /// <summary>
    ///     Gets or sets the variant status.
    /// </summary>
    public ProductStatus Status { get; set; }

    /// <summary>
    ///     Gets or sets the optional variant attributes.
    /// </summary>
    public List<VariantAttributeDocument>? Attributes { get; set; }

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
