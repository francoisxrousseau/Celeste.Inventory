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
using Microsoft.Extensions.Logging;

/// <summary>
///	Handles product update requests.
/// </summary>
public sealed class UpdateProductHandler(
    IProductRepository repository,
    ICurrentUserAccessor currentUserAccessor,
    IProductEventPublisher eventPublisher,
    IUnitOfWork unitOfWork,
    ILogger<UpdateProductHandler> logger)
    : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    /// <summary>
    ///	Handles the update product request.
    /// </summary>
    /// <param name="request">
    ///	The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///	The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///	The updated product response.
    /// </returns>
    public async Task<ProductResponse> HandleAsync(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _ = Product.NormalizeSearchText(request.Name)
            ?? throw new ArgumentException("Product name is required.", nameof(request));

        await using var transaction = await unitOfWork.BeginAsync(cancellationToken);
        var product = await repository.UpdateAsync(
            request.Id,
            request.ManufacturerId,
            request.Name.Trim(),
            request.Description?.Trim(),
            request.Status,
            request.Category,
            request.Tags,
            currentUserAccessor.UserId,
            request.UpdatedAt,
            cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException("Product was not found.");
        }

        await eventPublisher.PublishUpdatedAsync(product, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Updated product with ID {ProductId}.", product.Id);

        return product.ToResponse();
    }
}
