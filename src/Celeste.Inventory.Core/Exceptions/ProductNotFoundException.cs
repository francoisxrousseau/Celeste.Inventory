namespace Celeste.Inventory.Core.Exceptions;

/// <summary>
///     Represents a missing product in domain-facing workflows.
/// </summary>
public sealed class ProductNotFoundException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProductNotFoundException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The missing-product failure message.
    /// </param>
    public ProductNotFoundException(string message)
        : base(message)
    {
    }
}
