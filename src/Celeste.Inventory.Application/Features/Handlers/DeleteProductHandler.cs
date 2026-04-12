namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Commands;
using Celeste.Inventory.Core.Exceptions;
using Celeste.Inventory.Core.Identity;
using Celeste.Inventory.Core.Messaging;
using Celeste.Inventory.Core.Repositories;
using Emit.Abstractions;
using Emit.Mediator;
using Microsoft.Extensions.Logging;

/// <summary>
///	Handles product delete requests.
/// </summary>
public sealed class DeleteProductHandler(
    IProductRepository repository,
    ICurrentUserAccessor currentUserAccessor,
    IProductEventPublisher eventPublisher,
    IUnitOfWork unitOfWork,
    ILogger<DeleteProductHandler> logger)
    : IRequestHandler<DeleteProductCommand>
{
    /// <summary>
    ///	Handles the delete product request.
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
    public async Task HandleAsync(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction = await unitOfWork.BeginAsync(cancellationToken);
        var product = await repository.DeleteAsync(request.Id, currentUserAccessor.UserId, request.DeletedAt, cancellationToken);
        if (product is null)
            throw new ProductNotFoundException("Product was not found.");

        await eventPublisher.PublishDeletedAsync(
            product,
            currentUserAccessor.UserId,
            request.DeletedAt,
            cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Deleted product with ID {ProductId}.", product.Id);
    }
}
