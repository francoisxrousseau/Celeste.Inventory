namespace Celeste.Inventory.Infrastructure.Messaging;

using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Messaging;
using Celeste.Inventory.Events;
using Emit.Abstractions;
using Emit.MongoDB;

/// <summary>
///     Publishes manufacturer events through Emit's transactional outbox.
/// </summary>
public sealed class ManufacturerEventPublisher(
    IMongoSessionAccessor sessionAccessor,
    IEventProducer<string, ManufacturerEvent> producer)
    : IManufacturerEventPublisher
{
    /// <inheritdoc />
    public Task PublishCreatedAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default)
    {
        EnsureSession();

        return producer.ProduceAsync(
            manufacturer.Id.ToString(),
            ManufacturerEventFactory.Created(
                manufacturer.Id,
                manufacturer.Name,
                manufacturer.ContactEmail,
                manufacturer.ContactPhone,
                manufacturer.CreatedBy,
                manufacturer.CreatedAt),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task PublishUpdatedAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default)
    {
        EnsureSession();

        return producer.ProduceAsync(
            manufacturer.Id.ToString(),
            ManufacturerEventFactory.Updated(
                manufacturer.Id,
                manufacturer.Name,
                manufacturer.ContactEmail,
                manufacturer.ContactPhone,
                manufacturer.LastUpdatedBy,
                manufacturer.LastUpdatedAt ?? DateTime.UtcNow),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task PublishDeletedAsync(
        Manufacturer manufacturer,
        string? deletedBy,
        DateTime deletedAt,
        CancellationToken cancellationToken = default)
    {
        EnsureSession();

        return producer.ProduceAsync(
            manufacturer.Id.ToString(),
            ManufacturerEventFactory.Deleted(
                manufacturer.Id,
                manufacturer.Name,
                manufacturer.ContactEmail,
                manufacturer.ContactPhone,
                deletedBy,
                deletedAt),
            cancellationToken);
    }

    private void EnsureSession()
    {
        _ = sessionAccessor.Session ?? throw new InvalidOperationException("An active MongoDB session is required for manufacturer event publishing.");
    }
}
