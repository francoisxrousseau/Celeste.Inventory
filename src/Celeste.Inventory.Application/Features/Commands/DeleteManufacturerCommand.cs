namespace Celeste.Inventory.Application.Features.Commands;

using Emit.Mediator;

/// <summary>
///	Represents a request to soft delete a manufacturer.
/// </summary>
/// <param name="Id">
///	The manufacturer identifier.
/// </param>
/// <param name="DeletedAt">
///	The UTC timestamp for the operation.
/// </param>
public sealed record DeleteManufacturerCommand(
    Guid Id,
    DateTime DeletedAt) : IRequest;
