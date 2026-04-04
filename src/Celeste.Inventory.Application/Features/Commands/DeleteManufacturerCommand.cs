using Emit.Mediator;

namespace Celeste.Inventory.Application.Features.Commands;

/// <summary>
///	Represents a request to soft delete a manufacturer.
/// </summary>
/// <param name="Id">
///	The manufacturer identifier.
/// </param>
/// <param name="User">
///	The user responsible for the operation.
/// </param>
/// <param name="DeletedAt">
///	The UTC timestamp for the operation.
/// </param>
public sealed record DeleteManufacturerCommand(
    Guid Id,
    string? User,
    DateTime DeletedAt) : IRequest;
