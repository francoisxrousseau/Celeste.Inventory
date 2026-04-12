namespace Celeste.Inventory.Infrastructure.Messaging;

using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Messaging;
using Celeste.Inventory.Events;
using Emit.Abstractions;
using Emit.MongoDB;

/// <summary>
///     Publishes product events through Emit's transactional outbox.
/// </summary>
public sealed class ProductEventPublisher(
    IMongoSessionAccessor sessionAccessor,
    IEventProducer<string, ProductEvent> producer)
    : IProductEventPublisher
{
    /// <inheritdoc />
    public Task PublishCreatedAsync(Product product, CancellationToken cancellationToken = default)
    {
        EnsureSession();

        return producer.ProduceAsync(
            product.Id.ToString(),
            ProductEventFactory.Created(
                product.Id,
                product.ManufacturerId,
                product.Name,
                product.Description,
                product.Status.ToString(),
                product.Category.ToString(),
                product.Tags,
                product.CreatedBy,
                product.CreatedAt),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task PublishUpdatedAsync(Product product, CancellationToken cancellationToken = default)
    {
        EnsureSession();

        return producer.ProduceAsync(
            product.Id.ToString(),
            ProductEventFactory.Updated(
                product.Id,
                product.ManufacturerId,
                product.Name,
                product.Description,
                product.Status.ToString(),
                product.Category.ToString(),
                product.Tags,
                product.LastUpdatedBy,
                product.LastUpdatedAt ?? DateTime.UtcNow),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task PublishDeletedAsync(
        Product product,
        string? deletedBy,
        DateTime deletedAt,
        CancellationToken cancellationToken = default)
    {
        EnsureSession();

        return producer.ProduceAsync(
            product.Id.ToString(),
            ProductEventFactory.Deleted(
                product.Id,
                product.ManufacturerId,
                product.Name,
                product.Description,
                product.Status.ToString(),
                product.Category.ToString(),
                product.Tags,
                deletedBy,
                deletedAt),
            cancellationToken);
    }

    private void EnsureSession()
    {
        _ = sessionAccessor.Session ?? throw new InvalidOperationException("An active MongoDB session is required for product event publishing.");
    }
}
