namespace Celeste.Inventory.Core.Domain;

/// <summary>
///     Represents discount metadata associated with a variant.
/// </summary>
public sealed class DiscountInformations
{
    /// <summary>
    ///     Gets or sets the discount percentage.
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    ///     Gets or sets the UTC discount start timestamp.
    /// </summary>
    public DateTime DiscountStartAtUtc { get; set; }

    /// <summary>
    ///     Gets or sets the UTC discount end timestamp.
    /// </summary>
    public DateTime DiscountEndAtUtc { get; set; }
}
