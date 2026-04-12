namespace Celeste.Inventory.Api.Models.Products;

/// <summary>
///	Represents the query-string parameters for listing products.
/// </summary>
public sealed class ListProductsRequest
{
    /// <summary>
    ///	Gets or sets the 1-based page number.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    ///	Gets or sets the requested page size.
    /// </summary>
    public int PageSize { get; set; } = 25;

    /// <summary>
    ///	Gets or sets the optional search text.
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    ///	Gets or sets a value indicating whether deleted products are included.
    /// </summary>
    public bool IncludeDeleted { get; set; }

    /// <summary>
    ///	Gets or sets a value indicating whether the request is count-only.
    /// </summary>
    public bool CountOnly { get; set; }
}
