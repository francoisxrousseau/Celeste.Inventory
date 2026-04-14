namespace Celeste.Inventory.Api.Models.Products;

using Celeste.Inventory.Common.Enums;

/// <summary>
///     Represents the payload used to update a variant.
/// </summary>
public sealed class UpdateVariantRequest
{
    /// <summary>
    ///     Gets or sets the variant SKU.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the variant price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    ///     Gets or sets the optional discount information.
    /// </summary>
    public DiscountInformationsRequest? DiscountInformations { get; set; }

    /// <summary>
    ///     Gets or sets the variant status.
    /// </summary>
    public ProductStatus Status { get; set; }

    /// <summary>
    ///     Gets or sets the optional variant attributes.
    /// </summary>
    public List<VariantAttributeRequest>? Attributes { get; set; }
}
