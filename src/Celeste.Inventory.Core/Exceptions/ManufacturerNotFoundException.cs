namespace Celeste.Inventory.Core.Exceptions;

/// <summary>
///     Represents a missing manufacturer in domain-facing workflows.
/// </summary>
public sealed class ManufacturerNotFoundException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ManufacturerNotFoundException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The missing-manufacturer failure message.
    /// </param>
    public ManufacturerNotFoundException(string message)
        : base(message)
    {
    }
}
