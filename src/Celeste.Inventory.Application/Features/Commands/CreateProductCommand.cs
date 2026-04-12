namespace Celeste.Inventory.Application.Features.Commands;

using Celeste.Inventory.Common.Enums;
using Celeste.Inventory.Common.Responses;
using Emit.Mediator;

/// <summary>
///	Represents a request to create a product.
/// </summary>
/// <param name="ManufacturerId">
///	The manufacturer identifier.
/// </param>
/// <param name="Name">
///	The product name.
/// </param>
/// <param name="Description">
///	The optional product description.
/// </param>
/// <param name="Status">
///	The product status.
/// </param>
/// <param name="Category">
///	The product category.
/// </param>
/// <param name="Tags">
///	The optional product tags.
/// </param>
/// <param name="CreatedAt">
///	The UTC timestamp for the operation.
/// </param>
public sealed record CreateProductCommand(
    Guid ManufacturerId,
    string Name,
    string? Description,
    ProductStatus Status,
    ProductCategory Category,
    IReadOnlyList<string>? Tags,
    DateTime CreatedAt) : IRequest<ProductResponse>;
