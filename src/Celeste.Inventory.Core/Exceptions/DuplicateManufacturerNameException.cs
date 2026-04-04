namespace Celeste.Inventory.Core.Exceptions;

/// <summary>
///     Represents a duplicate manufacturer name conflict.
/// </summary>
public sealed class DuplicateManufacturerNameException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DuplicateManufacturerNameException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The duplicate-name failure message.
    /// </param>
    public DuplicateManufacturerNameException(string message)
        : base(message)
    {
    }
}
