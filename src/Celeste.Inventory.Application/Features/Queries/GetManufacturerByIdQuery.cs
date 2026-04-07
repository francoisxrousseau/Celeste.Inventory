namespace Celeste.Inventory.Application.Features.Queries;

using Celeste.Inventory.Common.Responses;
using Emit.Mediator;

/// <summary>
///	Represents a request to fetch a manufacturer by identifier.
/// </summary>
/// <param name="Id">
///	The manufacturer identifier.
/// </param>
/// <param name="AllowDeleted">
///	Indicates whether a deleted manufacturer may be returned.
/// </param>
public sealed record GetManufacturerByIdQuery(Guid Id, bool AllowDeleted) : IRequest<ManufacturerResponse>;
