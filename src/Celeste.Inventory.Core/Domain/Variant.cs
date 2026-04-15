namespace Celeste.Inventory.Core.Domain;

using Celeste.Inventory.Common.Enums;

/// <summary>
///     Represents a variant embedded within a product aggregate.
/// </summary>
public sealed class Variant : AuditableEntity
{
    /// <summary>
    ///     Gets or sets the variant identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the stock keeping unit.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the variant price. test
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    ///     Gets or sets the optional discount information.
    /// </summary>
    public DiscountInformations? DiscountInformations { get; set; }

    /// <summary>
    ///     Gets or sets the variant status.
    /// </summary>
    public ProductStatus Status { get; set; }

    /// <summary>
    ///     Gets or sets the optional variant attributes.
    /// </summary>
    public IReadOnlyList<VariantAttribute>? Attributes { get; set; }
}
