namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Commands;
using Celeste.Inventory.Core.Exceptions;
using Celeste.Inventory.Core.Identity;
using Celeste.Inventory.Core.Repositories;
using Emit.Mediator;

/// <summary>
///	Handles manufacturer delete requests.
/// </summary>
public sealed class DeleteManufacturerHandler(IManufacturerRepository repository, ICurrentUserAccessor currentUserAccessor)
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
        var deleted = await repository.DeleteAsync(request.Id, currentUserAccessor.UserId, request.DeletedAt, cancellationToken);
        if (!deleted)
        {
            throw new ManufacturerNotFoundException("Manufacturer was not found.");
        }
    }
}
