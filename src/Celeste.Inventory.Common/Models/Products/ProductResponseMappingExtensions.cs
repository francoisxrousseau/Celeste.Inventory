namespace Celeste.Inventory.Api.Models.Products;

/// <summary>
///     Provides mapping helpers from application product responses to API response models.
/// </summary>
public static class ProductResponseMappingExtensions
{
    /// <summary>
    ///     Maps an application product response to the API response model.
    /// </summary>
    /// <param name="response">
    ///     The application response to map.
    /// </param>
    /// <returns>
    ///     The API response model.
    /// </returns>
    public static ProductResponse ToApiModel(this Common.Responses.ProductResponse response)
    {
        return new ProductResponse(
            response.Id,
            response.ManufacturerId,
            response.Name,
            response.Description,
            response.Status,
            response.Tags);
    }

    /// <summary>
    ///     Maps an application product count response to the API response model.
    /// </summary>
    /// <param name="response">
    ///     The application response to map.
    /// </param>
    /// <returns>
    ///     The API response model.
    /// </returns>
    public static ProductCountResponse ToApiModel(this Common.Responses.ProductCountResponse response)
    {
        return new ProductCountResponse(response.TotalCount);
    }

    /// <summary>
    ///     Maps an application paged product response to the API response model.
    /// </summary>
    /// <param name="response">
    ///     The application response to map.
    /// </param>
    /// <returns>
    ///     The API response model.
    /// </returns>
    public static PagedProductsResponse ToApiModel(this Common.Responses.PagedProductsResponse response)
    {
        return new PagedProductsResponse(
            response.Items.Select(item => item.ToApiModel()).ToArray(),
            response.TotalCount,
            response.PageNumber,
            response.PageSize);
    }
}
