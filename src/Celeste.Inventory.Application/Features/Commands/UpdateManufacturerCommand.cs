using Celeste.Inventory.Common.Responses;
using Emit.Mediator;

namespace Celeste.Inventory.Application.Features.Commands;

/// <summary>
///	Represents a request to update a manufacturer.
/// </summary>
/// <param name="Id">
///	The manufacturer identifier.
/// </param>
/// <param name="Name">
///	The updated manufacturer name.
/// </param>
/// <param name="ContactEmail">
///	The updated optional contact email.
/// </param>
/// <param name="ContactPhone">
///	The updated optional contact phone number.
/// </param>
/// <param name="UpdatedAt">
///	The UTC timestamp for the operation.
/// </param>
public sealed record UpdateManufacturerCommand(
    Guid Id,
    string Name,
    string? ContactEmail,
    string? ContactPhone,
    DateTime UpdatedAt) : IRequest<ManufacturerResponse>;
