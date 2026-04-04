using Celeste.Inventory.Application.Features.Commands;
using Celeste.Inventory.Core.Exceptions;
using Celeste.Inventory.Core.Repositories;
using Emit.Mediator;

namespace Celeste.Inventory.Application.Features.Handlers;

/// <summary>
///	Handles manufacturer delete requests.
/// </summary>
public sealed class DeleteManufacturerHandler(IManufacturerRepository repository)
    : IRequestHandler<DeleteManufacturerCommand>
{
    /// <summary>
    ///	Handles the delete manufacturer request.
    /// </summary>
    /// <param name="request">
    ///	The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///	The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///	A task that completes when the delete has been applied.
    /// </returns>
    public async Task HandleAsync(DeleteManufacturerCommand request, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(request.Id, request.User, request.DeletedAt, cancellationToken);
        if (!deleted)
        {
            throw new ManufacturerNotFoundException("Manufacturer was not found.");
        }
    }
}
