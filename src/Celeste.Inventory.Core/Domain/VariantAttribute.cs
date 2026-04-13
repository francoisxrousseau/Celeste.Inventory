namespace Celeste.Inventory.Core.Domain;

/// <summary>
///     Represents a named variant attribute pair.
/// </summary>
public sealed class VariantAttribute
{
    /// <summary>
    ///     Gets or sets the attribute name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the attribute value.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
