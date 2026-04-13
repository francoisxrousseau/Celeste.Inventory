namespace Celeste.Inventory.Common.Responses;

/// <summary>
///     Represents variant discount data in a response contract.
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
