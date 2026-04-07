namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Commands;
using Celeste.Inventory.Application.Mapping;
using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Exceptions;
using Celeste.Inventory.Core.Identity;
using Celeste.Inventory.Core.Repositories;
using Emit.Mediator;

/// <summary>
///	Handles manufacturer creation requests.
/// </summary>
public sealed class CreateManufacturerHandler(IManufacturerRepository repository, ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<CreateManufacturerCommand, ManufacturerResponse>
{
    /// <summary>
    ///	Handles the create manufacturer request.
    /// </summary>
    /// <param name="request">
    ///	The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///	The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///	The created manufacturer response.
    /// </returns>
    public async Task<ManufacturerResponse> HandleAsync(CreateManufacturerCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = Manufacturer.NormalizeSearchText(request.Name)
            ?? throw new ArgumentException("Manufacturer name is required.", nameof(request));
        var trimmedName = request.Name.Trim();

        if (await repository.ExistsByNameAsync(normalizedName, cancellationToken: cancellationToken))
        {
            throw new DuplicateManufacturerNameException("A manufacturer with the same name already exists.");
        }

        var manufacturer = new Manufacturer
        {
            Id = Guid.NewGuid(),
            Name = trimmedName,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            CreatedBy = currentUserAccessor.UserId,
            CreatedAt = request.CreatedAt,
        };

        await repository.CreateAsync(manufacturer, cancellationToken);
        return manufacturer.ToResponse();
    }
}
