namespace Celeste.Inventory.Core.Messaging;

using Celeste.Inventory.Core.Domain;

/// <summary>
///     Defines application-facing product event publishing operations.
/// </summary>
public interface IProductEventPublisher
{
    /// <summary>
    ///     Publishes a product created event.
    /// </summary>
    /// <param name="product">
    ///     The created product.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that completes when the event has been enqueued.
    /// </returns>
    Task PublishCreatedAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Publishes a product updated event.
    /// </summary>
    /// <param name="product">
    ///     The updated product.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that completes when the event has been enqueued.
    /// </returns>
    Task PublishUpdatedAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Publishes a product deleted event.
    /// </summary>
    /// <param name="product">
    ///     The deleted product.
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
        Product product,
        string? deletedBy,
        DateTime deletedAt,
        CancellationToken cancellationToken = default);
}
