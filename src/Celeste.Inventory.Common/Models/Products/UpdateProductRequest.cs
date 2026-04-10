namespace Celeste.Inventory.Api.Models.Products;

using Celeste.Inventory.Common.Enums;

/// <summary>
///	Represents the payload used to update a product.
/// </summary>
public sealed class UpdateProductRequest
{
    /// <summary>
    ///     Gets or sets the manufacturer identifier.
    /// </summary>
    public Guid ManufacturerId { get; set; }

    /// <summary>
    ///	Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the optional product description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the product status.
    /// </summary>
    public ProductStatus Status { get; set; }

    /// <summary>
    ///     Gets or sets the optional product tags.
    /// </summary>
    public List<string>? Tags { get; set; }
}
