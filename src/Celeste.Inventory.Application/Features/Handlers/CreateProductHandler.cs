namespace Celeste.Inventory.Application.Features.Handlers;

using Celeste.Inventory.Application.Features.Commands;
using Celeste.Inventory.Application.Mapping;
using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Identity;
using Celeste.Inventory.Core.Messaging;
using Celeste.Inventory.Core.Repositories;
using Emit.Abstractions;
using Emit.Mediator;
using Microsoft.Extensions.Logging;

/// <summary>
///	Handles product creation requests.
/// </summary>
public sealed class CreateProductHandler(
    IProductRepository repository,
    ICurrentUserAccessor currentUserAccessor,
    IProductEventPublisher eventPublisher,
    IUnitOfWork unitOfWork,
    ILogger<CreateProductHandler> logger)
    : IRequestHandler<CreateProductCommand, ProductResponse>
{
    /// <summary>
    ///	Handles the create product request.
    /// </summary>
    /// <param name="request">
    ///	The request to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///	The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///	The created product response.
    /// </returns>
    public async Task<ProductResponse> HandleAsync(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _ = Product.NormalizeSearchText(request.Name)
            ?? throw new ArgumentException("Product name is required.", nameof(request));

        var product = new Product
        {
            Id = Guid.NewGuid(),
            ManufacturerId = request.ManufacturerId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Status = request.Status,
            Tags = request.Tags,
            CreatedBy = currentUserAccessor.UserId,
            CreatedAt = request.CreatedAt,
        };

        await using var transaction = await unitOfWork.BeginAsync(cancellationToken);
        await repository.CreateAsync(product, cancellationToken);
        await eventPublisher.PublishCreatedAsync(product, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Created product with ID {ProductId}.", product.Id);

        return product.ToResponse();
    }
}
