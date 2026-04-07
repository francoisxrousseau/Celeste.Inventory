namespace Celeste.Inventory.Api.Models.Manufacturers;

/// <summary>
///     Represents a count-only manufacturer API response.
/// </summary>
/// <param name="TotalCount">
///     The total number of matching manufacturers.
/// </param>
public sealed record ManufacturerCountResponse(long TotalCount);
