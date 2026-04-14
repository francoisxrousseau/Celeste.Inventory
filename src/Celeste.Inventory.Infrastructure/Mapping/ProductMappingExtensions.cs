namespace Celeste.Inventory.Infrastructure.Mapping;

using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Infrastructure.Documents;

/// <summary>
///     Provides mappings between product documents and domain entities.
/// </summary>
public static class ProductMappingExtensions
{
    /// <summary>
    ///     Maps a product document to a domain entity.
    /// </summary>
    /// <param name="document">
    ///     The document to map.
    /// </param>
    /// <returns>
    ///     The mapped product domain entity.
    /// </returns>
    public static Product ToDomain(this ProductDocument document)
    {
        return new Product
        {
            Id = document.Id,
            ManufacturerId = document.ManufacturerId,
            Name = document.Name,
            Description = document.Description,
            Status = document.Status,
            Category = document.Category,
            Tags = document.Tags,
            Variants = document.Variants?.Select(x => x.ToDomain()).ToList(),
            CreatedBy = document.CreatedBy,
            CreatedAt = document.CreatedAt,
            LastUpdatedBy = document.LastUpdatedBy,
            LastUpdatedAt = document.LastUpdatedAt,
            DeletedBy = document.DeletedBy,
            DeletedAt = document.DeletedAt,
        };
    }

    /// <summary>
    ///     Maps a product domain entity to a Mongo document.
    /// </summary>
    /// <param name="product">
    ///     The product to map.
    /// </param>
    /// <returns>
    ///     The mapped document.
    /// </returns>
    public static ProductDocument ToDocument(this Product product)
    {
        return new ProductDocument
        {
            Id = product.Id,
            ManufacturerId = product.ManufacturerId,
            Name = product.Name,
            NormalizedName = Product.NormalizeSearchText(product.Name) ?? string.Empty,
            Description = product.Description,
            Status = product.Status,
            Category = product.Category,
            Tags = product.Tags?.ToList(),
            Variants = product.Variants?.Select(x => x.ToDocument()).ToList(),
            CreatedBy = product.CreatedBy,
            CreatedAt = product.CreatedAt,
            LastUpdatedBy = product.LastUpdatedBy,
            LastUpdatedAt = product.LastUpdatedAt,
            DeletedBy = product.DeletedBy,
            DeletedAt = product.DeletedAt,
        };
    }

    /// <summary>
    ///     Maps a variant document to a domain entity.
    /// </summary>
    /// <param name="document">
    ///     The document to map.
    /// </param>
    /// <returns>
    ///     The mapped variant domain entity.
    /// </returns>
    public static Variant ToDomain(this VariantDocument document)
    {
        return new Variant
        {
            Id = document.Id,
            Sku = document.Sku,
            Price = document.Price,
            DiscountInformations = document.DiscountInformations?.ToDomain(),
            Status = document.Status,
            Attributes = document.Attributes?.Select(x => x.ToDomain()).ToList(),
            CreatedBy = document.CreatedBy,
            CreatedAt = document.CreatedAt,
            LastUpdatedBy = document.LastUpdatedBy,
            LastUpdatedAt = document.LastUpdatedAt,
            DeletedBy = document.DeletedBy,
            DeletedAt = document.DeletedAt,
        };
    }

    /// <summary>
    ///     Maps a variant domain entity to a Mongo document.
    /// </summary>
    /// <param name="variant">
    ///     The variant to map.
    /// </param>
    /// <returns>
    ///     The mapped document.
    /// </returns>
    public static VariantDocument ToDocument(this Variant variant)
    {
        return new VariantDocument
        {
            Id = variant.Id,
            Sku = variant.Sku,
            Price = variant.Price,
            DiscountInformations = variant.DiscountInformations?.ToDocument(),
            Status = variant.Status,
            Attributes = variant.Attributes?.Select(x => x.ToDocument()).ToList(),
            CreatedBy = variant.CreatedBy,
            CreatedAt = variant.CreatedAt,
            LastUpdatedBy = variant.LastUpdatedBy,
            LastUpdatedAt = variant.LastUpdatedAt,
            DeletedBy = variant.DeletedBy,
            DeletedAt = variant.DeletedAt,
        };
    }

    /// <summary>
    ///     Maps a discount document to a domain entity.
    /// </summary>
    /// <param name="document">
    ///     The document to map.
    /// </param>
    /// <returns>
    ///     The mapped discount domain entity.
    /// </returns>
    public static DiscountInformations ToDomain(this DiscountInformationsDocument document)
    {
        return new DiscountInformations
        {
            DiscountPercentage = document.DiscountPercentage,
            DiscountStartAtUtc = document.DiscountStartAtUtc,
            DiscountEndAtUtc = document.DiscountEndAtUtc,
        };
    }

    /// <summary>
    ///     Maps a discount domain entity to a Mongo document.
    /// </summary>
    /// <param name="discountInformations">
    ///     The discount entity to map.
    /// </param>
    /// <returns>
    ///     The mapped document.
    /// </returns>
    public static DiscountInformationsDocument ToDocument(this DiscountInformations discountInformations)
    {
        return new DiscountInformationsDocument
        {
            DiscountPercentage = discountInformations.DiscountPercentage,
            DiscountStartAtUtc = discountInformations.DiscountStartAtUtc,
            DiscountEndAtUtc = discountInformations.DiscountEndAtUtc,
        };
    }

    /// <summary>
    ///     Maps a variant attribute document to a domain entity.
    /// </summary>
    /// <param name="document">
    ///     The document to map.
    /// </param>
    /// <returns>
    ///     The mapped variant attribute domain entity.
    /// </returns>
    public static VariantAttribute ToDomain(this VariantAttributeDocument document)
    {
        return new VariantAttribute
        {
            Name = document.Name,
            Value = document.Value,
        };
    }

    /// <summary>
    ///     Maps a variant attribute domain entity to a Mongo document.
    /// </summary>
    /// <param name="variantAttribute">
    ///     The attribute to map.
    /// </param>
    /// <returns>
    ///     The mapped document.
    /// </returns>
    public static VariantAttributeDocument ToDocument(this VariantAttribute variantAttribute)
    {
        return new VariantAttributeDocument
        {
            Name = variantAttribute.Name,
            Value = variantAttribute.Value,
        };
    }
}
