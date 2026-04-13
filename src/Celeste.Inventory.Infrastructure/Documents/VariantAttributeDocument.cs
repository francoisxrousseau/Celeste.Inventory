namespace Celeste.Inventory.Infrastructure.Documents;

/// <summary>
///     Represents an embedded variant attribute document.
/// </summary>
public sealed class VariantAttributeDocument
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
