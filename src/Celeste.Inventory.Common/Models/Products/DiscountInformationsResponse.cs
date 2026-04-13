namespace Celeste.Inventory.Api.Models.Products;

/// <summary>
///     Represents variant discount data in the API response model.
/// </summary>
/// <param name="DiscountPercentage">
///     The discount percentage.
/// </param>
/// <param name="DiscountStartAtUtc">
///     The UTC discount start timestamp.
/// </param>
/// <param name="DiscountEndAtUtc">
///     The UTC discount end timestamp.
/// </param>
public sealed record DiscountInformationsResponse(
    decimal DiscountPercentage,
    DateTime DiscountStartAtUtc,
    DateTime DiscountEndAtUtc);
