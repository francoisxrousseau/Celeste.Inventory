namespace Celeste.Inventory.Common.Responses;

/// <summary>
///     Represents a paged product response.
/// </summary>
/// <param name="Items">
///     The product items for the requested page.
/// </param>
/// <param name="TotalCount">
///     The total number of matching products.
/// </param>
/// <param name="PageNumber">
///     The requested page number.
/// </param>
/// <param name="PageSize">
///     The requested page size.
/// </param>
public sealed record PagedProductsResponse(
    IReadOnlyList<ProductResponse> Items,
    long TotalCount,
    int PageNumber,
    int PageSize);
