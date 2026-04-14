namespace Celeste.Inventory.Api.Models.Products;

using Celeste.Inventory.Common.Enums;

/// <summary>
///     Represents a product variant returned by the product API.
/// </summary>
/// <param name="Id">
///     The variant identifier.
/// </param>
/// <param name="Sku">
///     The stock keeping unit.
/// </param>
/// <param name="Price">
///     The variant price.
/// </param>
/// <param name="DiscountInformations">
///     The optional discount information.
/// </param>
/// <param name="Status">
///     The variant status.
/// </param>
/// <param name="Attributes">
///     The optional variant attributes.
/// </param>
public sealed record VariantResponse(
    Guid Id,
    string Sku,
    decimal Price,
    DiscountInformationsResponse? DiscountInformations,
    ProductStatus Status,
    IReadOnlyList<VariantAttributeResponse>? Attributes);
