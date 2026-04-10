namespace Celeste.Inventory.Api.Models.Products;

/// <summary>
///     Represents a count-only product API response.
/// </summary>
/// <param name="TotalCount">
///     The total number of matching products.
/// </param>
public sealed record ProductCountResponse(long TotalCount);
