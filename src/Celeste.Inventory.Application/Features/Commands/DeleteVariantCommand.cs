namespace Celeste.Inventory.Application.Features.Commands;

using Emit.Mediator;

/// <summary>
///     Represents a request to soft delete a variant.
/// </summary>
/// <param name="ProductId">
///     The product identifier.
/// </param>
/// <param name="VariantId">
///     The variant identifier.
/// </param>
/// <param name="DeletedAt">
///     The UTC timestamp for the operation.
/// </param>
public sealed record DeleteVariantCommand(
    Guid ProductId,
    Guid VariantId,
    DateTime DeletedAt) : IRequest;
