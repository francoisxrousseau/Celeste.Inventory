namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Queries;
using Celeste.Inventory.Application.Mapping;
using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Repositories;
using Emit.Mediator;

/// <summary>
///	Handles paged product list queries.
/// </summary>
public sealed class ListProductsHandler(IProductRepository repository)
    : IRequestHandler<ListProductsQuery, PagedProductsResponse>
{
    /// <summary>
    ///	Handles the paged product list query.
    /// </summary>
    /// <param name="request">
    ///	The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///	The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///	The paged product response.
    /// </returns>
    public async Task<PagedProductsResponse> HandleAsync(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var normalizedSearchText = Product.NormalizeSearchText(request.SearchText);
        var items = await repository.SearchAsync(
            normalizedSearchText,
            request.PageNumber,
            request.PageSize,
            request.IncludeDeleted,
            cancellationToken);
        var totalCount = await repository.CountAsync(normalizedSearchText, request.IncludeDeleted, cancellationToken);

        return new PagedProductsResponse(
            items.Select(x => x.ToResponse()).ToList(),
            totalCount,
            request.PageNumber,
            request.PageSize);
    }
}
