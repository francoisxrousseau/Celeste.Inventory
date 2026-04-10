namespace Celeste.Inventory.Events;

/// <summary>
///     Provides helper operations for product event type values.
/// </summary>
public static class ProductEventTypeHelper
{
    /// <summary>
    ///     Determines whether the supplied event type is product-created.
    /// </summary>
    /// <param name="eventType">
    ///     The event type to evaluate.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> when the value is product-created; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsCreated(string? eventType)
    {
        return string.Equals(eventType, ProductEventTypes.Created, StringComparison.Ordinal);
    }

    /// <summary>
    ///     Determines whether the supplied event type is product-updated.
    /// </summary>
    /// <param name="eventType">
    ///     The event type to evaluate.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> when the value is product-updated; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsUpdated(string? eventType)
    {
        return string.Equals(eventType, ProductEventTypes.Updated, StringComparison.Ordinal);
    }

    /// <summary>
    ///     Determines whether the supplied event type is product-deleted.
    /// </summary>
    /// <param name="eventType">
    ///     The event type to evaluate.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> when the value is product-deleted; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsDeleted(string? eventType)
    {
        return string.Equals(eventType, ProductEventTypes.Deleted, StringComparison.Ordinal);
    }
}
