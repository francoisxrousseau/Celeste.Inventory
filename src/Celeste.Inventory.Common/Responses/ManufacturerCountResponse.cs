namespace Celeste.Inventory.Common.Responses;

/// <summary>
///	Represents a count-only manufacturer query response.
/// </summary>
/// <param name="TotalCount">
///	The total number of matching manufacturers.
/// </param>
public sealed record ManufacturerCountResponse(long TotalCount);
