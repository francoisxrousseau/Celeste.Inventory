namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Commands;
using Celeste.Inventory.Application.Mapping;
using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Exceptions;
using Celeste.Inventory.Core.Identity;
using Celeste.Inventory.Core.Messaging;
using Celeste.Inventory.Core.Repositories;
using Emit.Abstractions;
using Emit.Mediator;

/// <summary>
///	Handles manufacturer update requests.
/// </summary>
public sealed class UpdateManufacturerHandler(
    IManufacturerRepository repository,
    ICurrentUserAccessor currentUserAccessor,
    IManufacturerEventPublisher eventPublisher,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateManufacturerCommand, ManufacturerResponse>
{
    /// <summary>
    ///	Handles the update manufacturer request.
    /// </summary>
    /// <param name="request">
    ///	The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///	The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///	The updated manufacturer response.
    /// </returns>
    public async Task<ManufacturerResponse> HandleAsync(UpdateManufacturerCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = Manufacturer.NormalizeSearchText(request.Name)
            ?? throw new ArgumentException("Manufacturer name is required.", nameof(request));
        var trimmedName = request.Name.Trim();
        if (await repository.ExistsByNameAsync(normalizedName, request.Id, cancellationToken))
        {
            throw new DuplicateManufacturerNameException("A manufacturer with the same name already exists.");
        }

        await using var transaction = await unitOfWork.BeginAsync(cancellationToken);
        var manufacturer = await repository.UpdateAsync(
            request.Id,
            trimmedName,
            request.ContactEmail,
            request.ContactPhone,
            currentUserAccessor.UserId,
            request.UpdatedAt,
            cancellationToken);
        if (manufacturer is null)
        {
            throw new ManufacturerNotFoundException("Manufacturer was not found.");
        }

        await eventPublisher.PublishUpdatedAsync(manufacturer, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return manufacturer.ToResponse();
    }
}
