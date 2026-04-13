namespace Celeste.Inventory.Infrastructure.Documents;

/// <summary>
///     Represents embedded variant discount metadata.
/// </summary>
public sealed class DiscountInformationsDocument
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
