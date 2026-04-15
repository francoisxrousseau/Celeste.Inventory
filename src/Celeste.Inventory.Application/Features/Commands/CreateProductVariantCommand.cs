namespace Celeste.Inventory.Application.Features.Commands;

using Celeste.Inventory.Common.Enums;
using Celeste.Inventory.Core.Domain;

/// <summary>
///     Represents the optional initial variant for product creation.
/// </summary>
/// <param name="Sku">
///     The variant SKU.
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
public sealed record CreateProductVariantCommand(
    string Sku,
    decimal Price,
    DiscountInformations? DiscountInformations,
    ProductStatus Status,
    IReadOnlyList<VariantAttribute>? Attributes);
