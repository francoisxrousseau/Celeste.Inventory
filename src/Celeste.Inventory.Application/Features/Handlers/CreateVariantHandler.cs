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
///     Handles variant creation requests.
/// </summary>
public sealed class CreateVariantHandler(
    IProductRepository repository,
    ICurrentUserAccessor currentUserAccessor,
    IProductEventPublisher eventPublisher,
    IUnitOfWork unitOfWork,
    ILogger<CreateVariantHandler> logger)
    : IRequestHandler<CreateVariantCommand, VariantResponse>
{
    /// <summary>
    ///     Handles the create variant request.
    /// </summary>
    /// <param name="request">
    ///     The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the operation. test
    /// </param>
    /// <returns>
    ///     The created variant response.
    /// </returns>
    public async Task<VariantResponse> HandleAsync(
        CreateVariantCommand request,
        CancellationToken cancellationToken)
    {
        _ = Product.NormalizeSearchText(request.Sku)
            ?? throw new ArgumentException("Variant SKU is required.", nameof(request));

        var variant = new Variant
        {
            Id = Guid.NewGuid(),
            Sku = request.Sku.Trim(),
            Price = request.Price,
            DiscountInformations = request.DiscountInformations,
            Status = request.Status,
            Attributes = request.Attributes,
            CreatedBy = currentUserAccessor.UserId,
            CreatedAt = request.CreatedAt,
        };

        await using var transaction = await unitOfWork.BeginAsync(cancellationToken);
        var product = await repository.AddVariantAsync(
            request.ProductId,
            variant,
            currentUserAccessor.UserId,
            request.CreatedAt,
            cancellationToken);

        if (product is null)
            throw new ProductNotFoundException("Product was not found.");

        var created = product.Variants?.FirstOrDefault(x => x.Id == variant.Id)
            ?? throw new InvalidOperationException("Created variant was not returned by the repository.");

        await eventPublisher.PublishVariantCreatedAsync(product, created, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Created variant with ID {VariantId} for product {ProductId}.", created.Id, request.ProductId);

        return created.ToResponse();
    }
}
