namespace Celeste.Inventory.Application.Features.Queries;

using Celeste.Inventory.Common.Responses;
using Emit.Mediator;

/// <summary>
///     Represents a request to fetch all variants for a product.
/// </summary>
/// <param name="ProductId">
///     The product identifier.
/// </param>
/// <param name="IncludeDeleted">
///     Indicates whether deleted variants may be returned.
/// </param>
public sealed record ListVariantsQuery(
    Guid ProductId,
    bool IncludeDeleted) : IRequest<IReadOnlyList<VariantResponse>>;
