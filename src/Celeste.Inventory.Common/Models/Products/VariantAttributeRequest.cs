namespace Celeste.Inventory.Api.Models.Products;

/// <summary>
///     Represents a variant attribute payload item.
/// </summary>
public sealed class VariantAttributeRequest
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
