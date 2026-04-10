namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Queries;
using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Repositories;
using Emit.Mediator;

/// <summary>
///	Handles count-only product queries.
/// </summary>
public sealed class CountProductsHandler(IProductRepository repository)
    : IRequestHandler<CountProductsQuery, ProductCountResponse>
{
    /// <summary>
    ///	Handles the count-only product query.
    /// </summary>
    /// <param name="request">
    ///	The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///	The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///	The count-only response.
    /// </returns>
    public async Task<ProductCountResponse> HandleAsync(CountProductsQuery request, CancellationToken cancellationToken)
    {
        var normalizedSearchText = Product.NormalizeSearchText(request.SearchText);
        var totalCount = await repository.CountAsync(normalizedSearchText, request.IncludeDeleted, cancellationToken);
        return new ProductCountResponse(totalCount);
    }
}
