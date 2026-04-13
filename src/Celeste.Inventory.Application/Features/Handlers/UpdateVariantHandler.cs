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
///     Handles variant update requests.
/// </summary>
public sealed class UpdateVariantHandler(
    IProductRepository repository,
    ICurrentUserAccessor currentUserAccessor,
    IProductEventPublisher eventPublisher,
    IUnitOfWork unitOfWork,
    ILogger<UpdateVariantHandler> logger)
    : IRequestHandler<UpdateVariantCommand, VariantResponse>
{
    /// <summary>
    ///     Handles the update variant request.
    /// </summary>
    /// <param name="request">
    ///     The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///     The updated variant response.
    /// </returns>
    public async Task<VariantResponse> HandleAsync(
        UpdateVariantCommand request,
        CancellationToken cancellationToken)
    {
        _ = Product.NormalizeSearchText(request.Sku)
            ?? throw new ArgumentException("Variant SKU is required.", nameof(request));

        await using var transaction = await unitOfWork.BeginAsync(cancellationToken);
        var product = await repository.UpdateVariantAsync(
            request.ProductId,
            request.VariantId,
            request.Sku.Trim(),
            request.Price,
            request.DiscountInformations,
            request.Status,
            request.Attributes,
            currentUserAccessor.UserId,
            request.UpdatedAt,
            cancellationToken);

        if (product is null)
            throw new VariantNotFoundException("Variant was not found.");

        var updated = product.Variants?.FirstOrDefault(x => x.Id == request.VariantId)
            ?? throw new InvalidOperationException("Updated variant was not returned by the repository.");

        await eventPublisher.PublishVariantUpdatedAsync(product, updated, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Updated variant with ID {VariantId} for product {ProductId}.", updated.Id, request.ProductId);

        return updated.ToResponse();
    }
}
