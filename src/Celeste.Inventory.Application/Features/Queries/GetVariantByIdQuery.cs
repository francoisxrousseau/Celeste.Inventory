namespace Celeste.Inventory.Application.Features.Queries;

using Celeste.Inventory.Common.Responses;
using Emit.Mediator;

/// <summary>
///     Represents a request to fetch a variant by identifier.
/// </summary>
/// <param name="ProductId">
///     The product identifier.
/// </param>
/// <param name="VariantId">
///     The variant identifier.
/// </param>
/// <param name="AllowDeleted">
///     Indicates whether a deleted variant may be returned.
/// </param>
public sealed record GetVariantByIdQuery(
    Guid ProductId,
    Guid VariantId,
    bool AllowDeleted) : IRequest<VariantResponse>;
