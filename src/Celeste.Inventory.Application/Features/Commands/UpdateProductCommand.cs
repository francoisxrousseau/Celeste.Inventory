namespace Celeste.Inventory.Application.Features.Commands;

using Celeste.Inventory.Common.Enums;
using Celeste.Inventory.Common.Responses;
using Emit.Mediator;

/// <summary>
///	Represents a request to update a product.
/// </summary>
/// <param name="Id">
///	The product identifier.
/// </param>
/// <param name="ManufacturerId">
///	The manufacturer identifier.
/// </param>
/// <param name="Name">
///	The updated product name.
/// </param>
/// <param name="Description">
///	The updated optional product description.
/// </param>
/// <param name="Status">
///	The updated product status.
/// </param>
/// <param name="Tags">
///	The updated optional product tags.
/// </param>
/// <param name="UpdatedAt">
///	The UTC timestamp for the operation.
/// </param>
public sealed record UpdateProductCommand(
    Guid Id,
    Guid ManufacturerId,
    string Name,
    string? Description,
    ProductStatus Status,
    IReadOnlyList<string>? Tags,
    DateTime UpdatedAt) : IRequest<ProductResponse>;
