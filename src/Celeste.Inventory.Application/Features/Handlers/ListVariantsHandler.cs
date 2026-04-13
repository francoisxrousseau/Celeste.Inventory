namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Queries;
using Celeste.Inventory.Application.Mapping;
using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Exceptions;
using Celeste.Inventory.Core.Repositories;
using Emit.Mediator;

/// <summary>
///     Handles product variant list queries.
/// </summary>
public sealed class ListVariantsHandler(IProductRepository repository)
    : IRequestHandler<ListVariantsQuery, IReadOnlyList<VariantResponse>>
{
    /// <summary>
    ///     Handles the variant list query.
    /// </summary>
    /// <param name="request">
    ///     The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///     The matching variant responses.
    /// </returns>
    public async Task<IReadOnlyList<VariantResponse>> HandleAsync(
        ListVariantsQuery request,
        CancellationToken cancellationToken)
    {
        var variants = await repository.GetVariantsAsync(request.ProductId, request.IncludeDeleted, cancellationToken);
        return variants.Select(variant => variant.ToResponse()).ToList();
    }
}
