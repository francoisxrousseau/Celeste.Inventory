namespace Celeste.Inventory.Infrastructure.Messaging;

using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Messaging;
using Celeste.Inventory.Events;
using Emit.Abstractions;
using Emit.MongoDB;
using DomainVariant = Celeste.Inventory.Core.Domain.Variant;
using EventDiscountInformations = Celeste.Inventory.Events.DiscountInformations;
using EventVariant = Celeste.Inventory.Events.Variant;
using EventVariantAttribute = Celeste.Inventory.Events.VariantAttribute;

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

    /// <inheritdoc />
    public Task PublishVariantCreatedAsync(
        Product product,
        DomainVariant variant,
        CancellationToken cancellationToken = default)
    {
        EnsureSession();

        return producer.ProduceAsync(
            product.Id.ToString(),
            ProductEventFactory.VariantCreated(
                product.Id,
                product.ManufacturerId,
                product.Name,
                product.Description,
                product.Status.ToString(),
                product.Category.ToString(),
                product.Tags,
                MapVariant(variant),
                product.LastUpdatedBy ?? product.CreatedBy,
                product.LastUpdatedAt ?? product.CreatedAt),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task PublishVariantUpdatedAsync(
        Product product,
        DomainVariant variant,
        CancellationToken cancellationToken = default)
    {
        EnsureSession();

        return producer.ProduceAsync(
            product.Id.ToString(),
            ProductEventFactory.VariantUpdated(
                product.Id,
                product.ManufacturerId,
                product.Name,
                product.Description,
                product.Status.ToString(),
                product.Category.ToString(),
                product.Tags,
                MapVariant(variant),
                product.LastUpdatedBy,
                product.LastUpdatedAt ?? DateTime.UtcNow),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task PublishVariantDeletedAsync(
        Product product,
        DomainVariant variant,
        string? deletedBy,
        DateTime deletedAt,
        CancellationToken cancellationToken = default)
    {
        EnsureSession();

        return producer.ProduceAsync(
            product.Id.ToString(),
            ProductEventFactory.VariantDeleted(
                product.Id,
                product.ManufacturerId,
                product.Name,
                product.Description,
                product.Status.ToString(),
                product.Category.ToString(),
                product.Tags,
                MapVariant(variant),
                deletedBy,
                deletedAt),
            cancellationToken);
    }

    private void EnsureSession()
    {
        _ = sessionAccessor.Session ?? throw new InvalidOperationException("An active MongoDB session is required for product event publishing.");
    }

    private static EventVariant MapVariant(DomainVariant variant)
    {
        return new EventVariant
        {
            Id = variant.Id,
            Sku = variant.Sku,
            Price = AvroDecimalSerializer.Serialize(variant.Price),
            Discount = variant.DiscountInformations is null
                ? null
                : new EventDiscountInformations
                {
                    DiscountPercentage = AvroDecimalSerializer.Serialize(variant.DiscountInformations.DiscountPercentage),
                    DiscountStartAtUtc = variant.DiscountInformations.DiscountStartAtUtc,
                    DiscountEndAtUtc = variant.DiscountInformations.DiscountEndAtUtc,
                },
            Status = variant.Status.ToString(),
            Attributes = variant.Attributes?.Select(attribute => new EventVariantAttribute
            {
                Name = attribute.Name,
                Value = attribute.Value,
            }).ToList(),
        };
    }
}
