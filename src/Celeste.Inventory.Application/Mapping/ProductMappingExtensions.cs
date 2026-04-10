namespace Celeste.Inventory.Application.Mapping;

using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Domain;

/// <summary>
///	Provides mapping helpers for product responses.
/// </summary>
public static class ProductMappingExtensions
{
    /// <summary>
    ///	Maps a product entity to its response contract.
    /// </summary>
    /// <param name="product">
    ///	The product to map.
    /// </param>
    /// <returns>
    ///	The mapped response.
    /// </returns>
    public static ProductResponse ToResponse(this Product product)
    {
        return new ProductResponse(
            product.Id,
            product.ManufacturerId,
            product.Name,
            product.Description,
            product.Status,
            product.Tags);
    }
}
