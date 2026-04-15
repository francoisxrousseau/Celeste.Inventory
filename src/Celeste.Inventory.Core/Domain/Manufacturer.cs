namespace Celeste.Inventory.Core.Domain;

/// <summary>
///     Represents a manufacturer associated with products in the catalog.
/// </summary>
public sealed class Manufacturer : AuditableEntity
{
    /// <summary>
    ///     Gets the unique identifier of the manufacturer.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets the display name of the manufacturer.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets the contact email for the manufacturer.
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    ///     Gets the contact phone number for the manufacturer.
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    ///     Normalizes free-text input for deterministic manufacturer comparisons.
    /// </summary>
    /// <param name="value">
    ///     The value to normalize.
    /// </param>
    /// <returns>
    ///     The trimmed invariant-cased value, or <see langword="null"/> when the input is empty.
    /// </returns>
    public static string? NormalizeSearchText(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed.ToUpperInvariant();
    }
}
