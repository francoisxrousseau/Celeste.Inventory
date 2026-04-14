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
///     Handles variant delete requests.
/// </summary>
public sealed class DeleteVariantHandler(
    IProductRepository repository,
    ICurrentUserAccessor currentUserAccessor,
    IProductEventPublisher eventPublisher,
    IUnitOfWork unitOfWork,
    ILogger<DeleteVariantHandler> logger)
    : IRequestHandler<DeleteVariantCommand>
{
    /// <summary>
    ///     Handles the delete variant request.
    /// </summary>
    /// <param name="request">
    ///     The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///     A task that completes when the delete has been applied.
    /// </returns>
    public async Task HandleAsync(
        DeleteVariantCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction = await unitOfWork.BeginAsync(cancellationToken);
        var product = await repository.DeleteVariantAsync(
            request.ProductId,
            request.VariantId,
            currentUserAccessor.UserId,
            request.DeletedAt,
            cancellationToken);

        if (product is null)
            throw new VariantNotFoundException("Variant was not found.");

        var deleted = product.Variants?.FirstOrDefault(x => x.Id == request.VariantId)
            ?? throw new InvalidOperationException("Deleted variant was not returned by the repository.");

        await eventPublisher.PublishVariantDeletedAsync(
            product,
            deleted,
            currentUserAccessor.UserId,
            request.DeletedAt,
            cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Deleted variant with ID {VariantId} for product {ProductId}.", deleted.Id, request.ProductId);
    }
}
