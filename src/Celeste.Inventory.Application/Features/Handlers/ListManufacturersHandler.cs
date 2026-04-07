namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Queries;
using Celeste.Inventory.Application.Mapping;
using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Repositories;
using Emit.Mediator;

/// <summary>
///	Handles paged manufacturer list queries.
/// </summary>
public sealed class ListManufacturersHandler(IManufacturerRepository repository)
    : IRequestHandler<ListManufacturersQuery, PagedManufacturersResponse>
{
    /// <summary>
    ///	Handles the paged manufacturer list query.
    /// </summary>
    /// <param name="request">
    ///	The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///	The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///	The paged manufacturer response.
    /// </returns>
    public async Task<PagedManufacturersResponse> HandleAsync(ListManufacturersQuery request, CancellationToken cancellationToken)
    {
        var normalizedSearchText = Manufacturer.NormalizeSearchText(request.SearchText);
        var items = await repository.SearchAsync(
            normalizedSearchText,
            request.PageNumber,
            request.PageSize,
            request.IncludeDeleted,
            cancellationToken);
        var totalCount = await repository.CountAsync(normalizedSearchText, request.IncludeDeleted, cancellationToken);

        return new PagedManufacturersResponse(
            items.Select(x => x.ToResponse()).ToList(),
            totalCount,
            request.PageNumber,
            request.PageSize);
    }
}
