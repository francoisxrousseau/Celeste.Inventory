namespace Celeste.Inventory.Api.Models.Products;

using Celeste.Inventory.Common.Enums;

/// <summary>
///     Represents a product returned by the product API.
/// </summary>
/// <param name="Id">
///     The product identifier.
/// </param>
/// <param name="ManufacturerId">
///     The manufacturer identifier.
/// </param>
/// <param name="Name">
///     The product display name.
/// </param>
/// <param name="Description">
///     The optional product description.
/// </param>
/// <param name="Status">
///     The product status.
/// </param>
/// <param name="Category">
///     The product category.
/// </param>
/// <param name="Tags">
///     The optional product tags.
/// </param>
public sealed record ProductResponse(
    Guid Id,
    Guid ManufacturerId,
    string Name,
    string? Description,
    ProductStatus Status,
    ProductCategory Category,
    IReadOnlyList<string>? Tags);
