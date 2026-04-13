namespace Celeste.Inventory.Application.Features.Commands;

using Celeste.Inventory.Common.Enums;
using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Domain;
using Emit.Mediator;

/// <summary>
///     Represents a request to create a variant.
/// </summary>
/// <param name="ProductId">
///     The product identifier.
/// </param>
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
/// <param name="CreatedAt">
///     The UTC timestamp for the operation.
/// </param>
public sealed record CreateVariantCommand(
    Guid ProductId,
    string Sku,
    decimal Price,
    DiscountInformations? DiscountInformations,
    ProductStatus Status,
    IReadOnlyList<VariantAttribute>? Attributes,
    DateTime CreatedAt) : IRequest<VariantResponse>;
