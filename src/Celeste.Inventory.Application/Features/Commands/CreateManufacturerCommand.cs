using Celeste.Inventory.Common.Responses;
using Emit.Mediator;

namespace Celeste.Inventory.Application.Features.Commands;

/// <summary>
///	Represents a request to create a manufacturer.
/// </summary>
/// <param name="Name">
///	The manufacturer name.
/// </param>
/// <param name="ContactEmail">
///	The optional contact email.
/// </param>
/// <param name="ContactPhone">
///	The optional contact phone number.
/// </param>
/// <param name="User">
///	The user responsible for the operation.
/// </param>
/// <param name="CreatedAt">
///	The UTC timestamp for the operation.
/// </param>
public sealed record CreateManufacturerCommand(
    string Name,
    string? ContactEmail,
    string? ContactPhone,
    string? User,
    DateTime CreatedAt) : IRequest<ManufacturerResponse>;
