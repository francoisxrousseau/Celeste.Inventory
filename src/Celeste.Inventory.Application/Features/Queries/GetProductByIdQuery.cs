namespace Celeste.Inventory.Application.Features.Queries;

using Celeste.Inventory.Common.Responses;
using Emit.Mediator;

/// <summary>
///	Represents a request to fetch a product by identifier.
/// </summary>
/// <param name="Id">
///	The product identifier.
/// </param>
/// <param name="AllowDeleted">
///	Indicates whether a deleted product may be returned.
/// </param>
public sealed record GetProductByIdQuery(Guid Id, bool AllowDeleted) : IRequest<ProductResponse>;
