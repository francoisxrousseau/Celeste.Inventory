namespace Celeste.Inventory.Core.Exceptions;

/// <summary>
///     Represents an error raised when a variant cannot be found.
/// </summary>
public sealed class VariantNotFoundException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="VariantNotFoundException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The exception message.
    /// </param>
    public VariantNotFoundException(string message)
        : base(message)
    {
    }
}
