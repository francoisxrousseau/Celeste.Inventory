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
            Tags = document.Tags,
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
            Tags = product.Tags?.ToList(),
            CreatedBy = product.CreatedBy,
            CreatedAt = product.CreatedAt,
            LastUpdatedBy = product.LastUpdatedBy,
            LastUpdatedAt = product.LastUpdatedAt,
            DeletedBy = product.DeletedBy,
            DeletedAt = product.DeletedAt,
        };
    }
}
