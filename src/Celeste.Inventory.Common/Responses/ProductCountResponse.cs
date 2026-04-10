namespace Celeste.Inventory.Common.Responses;

/// <summary>
///     Represents a count-only product response.
/// </summary>
/// <param name="TotalCount">
///     The total number of matching products.
/// </param>
public sealed record ProductCountResponse(long TotalCount);
