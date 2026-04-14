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

    /// <summary>
    ///     Publishes a variant created event.
    /// </summary>
    /// <param name="product">
    ///     The product that owns the variant.
    /// </param>
    /// <param name="variant">
    ///     The created variant.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that completes when the event has been enqueued.
    /// </returns>
    Task PublishVariantCreatedAsync(
        Product product,
        Variant variant,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Publishes a variant updated event.
    /// </summary>
    /// <param name="product">
    ///     The product that owns the variant.
    /// </param>
    /// <param name="variant">
    ///     The updated variant.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task that completes when the event has been enqueued.
    /// </returns>
    Task PublishVariantUpdatedAsync(
        Product product,
        Variant variant,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Publishes a variant deleted event.
    /// </summary>
    /// <param name="product">
    ///     The product that owns the variant.
    /// </param>
    /// <param name="variant">
    ///     The deleted variant.
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
    Task PublishVariantDeletedAsync(
        Product product,
        Variant variant,
        string? deletedBy,
        DateTime deletedAt,
        CancellationToken cancellationToken = default);
}
