namespace Celeste.Inventory.Core.Messaging;

using Celeste.Inventory.Core.Domain;

/// <summary>
///     Defines application-facing manufacturer event publishing operations.
/// </summary>
public interface IManufacturerEventPublisher
{
    /// <summary>
    ///     Publishes a manufacturer created event.
    /// </summary>
    /// <param name="manufacturer">
    ///     The created manufacturer.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that completes when the event has been enqueued.
    /// </returns>
    Task PublishCreatedAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Publishes a manufacturer updated event.
    /// </summary>
    /// <param name="manufacturer">
    ///     The updated manufacturer.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that completes when the event has been enqueued.
    /// </returns>
    Task PublishUpdatedAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Publishes a manufacturer deleted event.
    /// </summary>
    /// <param name="manufacturer">
    ///     The deleted manufacturer.
    /// </param>
    /// <param name="deletedBy">
    ///     The user responsible for the deletion.
    /// </param>
    /// <param name="deletedAt">
    ///     The UTC deletion timestamp.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that completes when the event has been enqueued.
    /// </returns>
    Task PublishDeletedAsync(
        Manufacturer manufacturer,
        string? deletedBy,
        DateTime deletedAt,
        CancellationToken cancellationToken = default);
}
