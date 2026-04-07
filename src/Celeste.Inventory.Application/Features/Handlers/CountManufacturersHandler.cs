namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Queries;
using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Repositories;
using Emit.Mediator;

/// <summary>
///	Handles count-only manufacturer queries.
/// </summary>
public sealed class CountManufacturersHandler(IManufacturerRepository repository)
    : IRequestHandler<CountManufacturersQuery, ManufacturerCountResponse>
{
    /// <summary>
    ///	Handles the count-only manufacturer query.
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
    public async Task<ManufacturerCountResponse> HandleAsync(CountManufacturersQuery request, CancellationToken cancellationToken)
    {
        var normalizedSearchText = Manufacturer.NormalizeSearchText(request.SearchText);
        var totalCount = await repository.CountAsync(normalizedSearchText, request.IncludeDeleted, cancellationToken);
        return new ManufacturerCountResponse(totalCount);
    }
}
