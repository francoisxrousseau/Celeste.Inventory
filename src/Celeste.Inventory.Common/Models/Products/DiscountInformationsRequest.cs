namespace Celeste.Inventory.Api.Models.Products;

/// <summary>
///     Represents the discount information payload for a variant.
/// </summary>
public sealed class DiscountInformationsRequest
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
