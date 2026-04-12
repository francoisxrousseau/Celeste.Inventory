namespace Celeste.Inventory.Core.Domain;

using Celeste.Inventory.Common.Enums;

/// <summary>
///     Represents a product in the catalog.
/// </summary>
public sealed class Product : AuditableEntity
{
    /// <summary>
    ///     Gets or sets the product identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the manufacturer identifier.
    /// </summary>
    public Guid ManufacturerId { get; set; }

    /// <summary>
    ///     Gets or sets the product display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the optional product description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the product status.
    /// </summary>
    public ProductStatus Status { get; set; }

    /// <summary>
    ///     Gets or sets the product category.
    /// </summary>
    public ProductCategory Category { get; set; }

    /// <summary>
    ///     Gets or sets the optional product tags.
    /// </summary>
    public IReadOnlyList<string>? Tags { get; set; }

    /// <summary>
    ///     Normalizes free-text input for deterministic product comparisons.
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
