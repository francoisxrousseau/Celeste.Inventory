namespace Celeste.Inventory.Application.Features.Commands;

using Emit.Mediator;

/// <summary>
///	Represents a request to soft delete a product.
/// </summary>
/// <param name="Id">
///	The product identifier.
/// </param>
/// <param name="DeletedAt">
///	The UTC timestamp for the operation.
/// </param>
public sealed record DeleteProductCommand(
    Guid Id,
    DateTime DeletedAt) : IRequest;
