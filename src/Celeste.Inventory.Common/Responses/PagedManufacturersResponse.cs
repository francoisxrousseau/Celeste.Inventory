namespace Celeste.Inventory.Common.Responses;

/// <summary>
///	Represents a paged manufacturer query response.
/// </summary>
/// <param name="Items">
///	The manufacturer items for the requested page.
/// </param>
/// <param name="TotalCount">
///	The total number of matching manufacturers.
/// </param>
/// <param name="PageNumber">
///	The requested page number.
/// </param>
/// <param name="PageSize">
///	The requested page size.
/// </param>
public sealed record PagedManufacturersResponse(
    IReadOnlyList<ManufacturerResponse> Items,
    long TotalCount,
    int PageNumber,
    int PageSize);
