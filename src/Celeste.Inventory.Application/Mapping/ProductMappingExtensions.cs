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
            product.Category,
            product.Tags,
            product.Variants?
                .Where(variant => !variant.IsDeleted)
                .Select(variant => variant.ToResponse())
                .ToList());
    }

    /// <summary>
    ///     Maps a variant entity to its response contract.
    /// </summary>
    /// <param name="variant">
    ///     The variant to map.
    /// </param>
    /// <returns>
    ///     The mapped response.
    /// </returns>
    public static VariantResponse ToResponse(this Variant variant)
    {
        return new VariantResponse(
            variant.Id,
            variant.Sku,
            variant.Price,
            variant.DiscountInformations?.ToResponse(),
            variant.Status,
            variant.Attributes?.Select(attribute => attribute.ToResponse()).ToList());
    }

    /// <summary>
    ///     Maps a discount entity to its response contract.
    /// </summary>
    /// <param name="discountInformations">
    ///     The discount information to map.
    /// </param>
    /// <returns>
    ///     The mapped response.
    /// </returns>
    public static DiscountInformationsResponse ToResponse(this DiscountInformations discountInformations)
    {
        return new DiscountInformationsResponse(
            discountInformations.DiscountPercentage,
            discountInformations.DiscountStartAtUtc,
            discountInformations.DiscountEndAtUtc);
    }

    /// <summary>
    ///     Maps a variant attribute entity to its response contract.
    /// </summary>
    /// <param name="variantAttribute">
    ///     The attribute to map.
    /// </param>
    /// <returns>
    ///     The mapped response.
    /// </returns>
    public static VariantAttributeResponse ToResponse(this VariantAttribute variantAttribute)
    {
        return new VariantAttributeResponse(
            variantAttribute.Name,
            variantAttribute.Value);
    }
}
