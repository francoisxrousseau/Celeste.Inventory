namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Queries;
using Celeste.Inventory.Application.Mapping;
using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Exceptions;
using Celeste.Inventory.Core.Repositories;
using Emit.Mediator;

/// <summary>
///	Handles manufacturer by-id queries.
/// </summary>
public sealed class GetManufacturerByIdHandler(IManufacturerRepository repository)
    : IRequestHandler<GetManufacturerByIdQuery, ManufacturerResponse>
{
    /// <summary>
    ///	Handles the by-id manufacturer query.
    /// </summary>
    /// <param name="request">
    ///	The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///	The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///	The matching manufacturer response.
    /// </returns>
    public async Task<ManufacturerResponse> HandleAsync(GetManufacturerByIdQuery request, CancellationToken cancellationToken)
    {
        var manufacturer = await repository.GetByIdAsync(request.Id, request.AllowDeleted, cancellationToken);
        if (manufacturer is null)
        {
            throw new ManufacturerNotFoundException("Manufacturer was not found.");
        }

        return manufacturer.ToResponse();
    }
}
