namespace Celeste.Inventory.Common.Responses;

/// <summary>
///     Represents a variant attribute response item.
/// </summary>
/// <param name="Name">
///     The attribute name.
/// </param>
/// <param name="Value">
///     The attribute value.
/// </param>
public sealed record VariantAttributeResponse(
    string Name,
    string Value);
