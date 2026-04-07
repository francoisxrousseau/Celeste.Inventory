namespace Celeste.Inventory.Api.Models.Manufacturers;

/// <summary>
///     Represents a paged manufacturer API response.
/// </summary>
/// <param name="Items">
///     The manufacturer items for the requested page.
/// </param>
/// <param name="TotalCount">
///     The total number of matching manufacturers.
/// </param>
/// <param name="PageNumber">
///     The requested page number.
/// </param>
/// <param name="PageSize">
///     The requested page size.
/// </param>
public sealed record PagedManufacturersResponse(
    IReadOnlyList<ManufacturerResponse> Items,
    long TotalCount,
    int PageNumber,
    int PageSize);
