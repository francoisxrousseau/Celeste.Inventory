namespace Celeste.Inventory.Infrastructure.Repositories;

using Celeste.Inventory.Common.Enums;
using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Repositories;
using Celeste.Inventory.Infrastructure.Documents;
using Celeste.Inventory.Infrastructure.Mapping;
using MongoDB.Bson;
using Emit.MongoDB;
using MongoDB.Driver;
using System.Text.RegularExpressions;

/// <summary>
///     Provides MongoDB persistence for products.
/// </summary>
public sealed class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<ProductDocument> _collection;
    private readonly IMongoSessionAccessor _sessionAccessor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProductRepository"/> class.
    /// </summary>
    /// <param name="database">
    ///     The Mongo database used for product persistence.
    /// </param>
    /// <param name="sessionAccessor">
    ///     Provides access to the active MongoDB session.
    /// </param>
    public ProductRepository(
        IMongoDatabase database,
        IMongoSessionAccessor sessionAccessor)
    {
        _collection = database.GetCollection<ProductDocument>("products");
        _sessionAccessor = sessionAccessor;
    }

    /// <inheritdoc />
    public Task<long> CountAsync(string? searchText, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        return _collection.CountDocumentsAsync(BuildSearchFilter(searchText, includeDeleted), cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        var document = product.ToDocument();
        return _collection.InsertOneAsync(GetRequiredSession(), document, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Product?> AddVariantAsync(
        Guid productId,
        Variant variant,
        string? createdBy,
        DateTime createdAt,
        CancellationToken cancellationToken = default)
    {
        var filter = BuildActiveProductFilter(productId);
        var update = BuildAppendVariantUpdate(variant.ToDocument(), createdBy, createdAt);
        var options = new FindOneAndUpdateOptions<ProductDocument>
        {
            ReturnDocument = ReturnDocument.After,
        };

        var document = await _collection.FindOneAndUpdateAsync(GetRequiredSession(), filter, update, options, cancellationToken);
        return document?.ToDomain();
    }

    /// <inheritdoc />
    public async Task<Product?> DeleteAsync(Guid id, string? deletedBy, DateTime deletedAt, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ProductDocument>.Filter.Eq(x => x.Id, id) &
            Builders<ProductDocument>.Filter.Eq(x => x.DeletedAt, null as DateTime?);
        var update = Builders<ProductDocument>.Update
            .Set(x => x.DeletedBy, deletedBy)
            .Set(x => x.DeletedAt, deletedAt);
        var options = new FindOneAndUpdateOptions<ProductDocument>
        {
            ReturnDocument = ReturnDocument.After,
        };

        var document = await _collection.FindOneAndUpdateAsync(GetRequiredSession(), filter, update, options, cancellationToken);
        return document?.ToDomain();
    }

    /// <inheritdoc />
    public async Task<Product?> DeleteVariantAsync(
        Guid productId,
        Guid variantId,
        string? deletedBy,
        DateTime deletedAt,
        CancellationToken cancellationToken = default)
    {
        var filter = BuildActiveProductWithVariantFilter(productId, variantId);
        var update = Builders<ProductDocument>.Update
            .Set("Variants.$.DeletedBy", deletedBy)
            .Set("Variants.$.DeletedAt", deletedAt)
            .Set(x => x.LastUpdatedBy, deletedBy)
            .Set(x => x.LastUpdatedAt, deletedAt);
        var options = new FindOneAndUpdateOptions<ProductDocument>
        {
            ReturnDocument = ReturnDocument.After,
        };

        var document = await _collection.FindOneAndUpdateAsync(GetRequiredSession(), filter, update, options, cancellationToken);
        return document?.ToDomain();
    }

    /// <inheritdoc />
    public async Task<Product?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ProductDocument>.Filter.Eq(x => x.Id, id);
        if (!includeDeleted)
        {
            filter &= Builders<ProductDocument>.Filter.Eq(x => x.DeletedAt, null as DateTime?);
        }

        var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (document is null)
        {
            return null;
        }

        return document.ToDomain();
    }

    /// <inheritdoc />
    public async Task<Variant?> GetVariantByIdAsync(
        Guid productId,
        Guid variantId,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var document = await _collection.Find(BuildActiveProductFilter(productId))
            .FirstOrDefaultAsync(cancellationToken);

        if (document?.Variants is null)
            return null;

        var variant = document.Variants.FirstOrDefault(x => x.Id == variantId);
        if (variant is null || (!includeDeleted && variant.DeletedAt is not null))
            return null;

        return variant.ToDomain();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Variant>> GetVariantsAsync(
        Guid productId,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var document = await _collection.Find(BuildActiveProductFilter(productId))
            .FirstOrDefaultAsync(cancellationToken);

        if (document?.Variants is null)
            return [];

        var variants = includeDeleted
            ? document.Variants
            : document.Variants.Where(x => x.DeletedAt is null);

        return variants.Select(x => x.ToDomain()).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Product>> SearchAsync(string? searchText, int pageNumber, int pageSize, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var documents = await _collection.Find(BuildSearchFilter(searchText, includeDeleted))
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return documents.Select(x => x.ToDomain()).ToList();
    }

    /// <inheritdoc />
    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ProductDocument>.Filter.Eq(x => x.Id, product.Id);
        var update = Builders<ProductDocument>.Update
            .Set(x => x.ManufacturerId, product.ManufacturerId)
            .Set(x => x.Name, product.Name)
            .Set(x => x.NormalizedName, Product.NormalizeSearchText(product.Name) ?? string.Empty)
            .Set(x => x.Description, product.Description)
            .Set(x => x.Status, product.Status)
            .Set(x => x.Category, product.Category)
            .Set(x => x.Tags, product.Tags?.ToList())
            .Set(x => x.LastUpdatedBy, product.LastUpdatedBy)
            .Set(x => x.LastUpdatedAt, product.LastUpdatedAt)
            .Set(x => x.DeletedBy, product.DeletedBy)
            .Set(x => x.DeletedAt, product.DeletedAt);

        return _collection.UpdateOneAsync(GetRequiredSession(), filter, update, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Product?> UpdateAsync(
        Guid id,
        Guid manufacturerId,
        string name,
        string? description,
        ProductStatus status,
        ProductCategory category,
        IReadOnlyList<string>? tags,
        string? updatedBy,
        DateTime updatedAt,
        CancellationToken cancellationToken = default)
    {
        var trimmedName = name.Trim();
        var normalizedName = Product.NormalizeSearchText(name) ?? string.Empty;
        var filter = Builders<ProductDocument>.Filter.Eq(x => x.Id, id) &
            Builders<ProductDocument>.Filter.Eq(x => x.DeletedAt, null as DateTime?);
        var update = Builders<ProductDocument>.Update
            .Set(x => x.ManufacturerId, manufacturerId)
            .Set(x => x.Name, trimmedName)
            .Set(x => x.NormalizedName, normalizedName)
            .Set(x => x.Description, description)
            .Set(x => x.Status, status)
            .Set(x => x.Category, category)
            .Set(x => x.Tags, tags == null ? null : tags.ToList())
            .Set(x => x.LastUpdatedBy, updatedBy)
            .Set(x => x.LastUpdatedAt, updatedAt);

        var options = new FindOneAndUpdateOptions<ProductDocument>
        {
            ReturnDocument = ReturnDocument.After,
        };
        var document = await _collection.FindOneAndUpdateAsync(GetRequiredSession(), filter, update, options, cancellationToken);

        return document?.ToDomain();
    }

    /// <inheritdoc />
    public async Task<Product?> UpdateVariantAsync(
        Guid productId,
        Guid variantId,
        string sku,
        decimal price,
        DiscountInformations? discountInformations,
        ProductStatus status,
        IReadOnlyList<VariantAttribute>? attributes,
        string? updatedBy,
        DateTime updatedAt,
        CancellationToken cancellationToken = default)
    {
        var filter = BuildActiveProductWithVariantFilter(productId, variantId);
        var update = Builders<ProductDocument>.Update
            .Set("Variants.$.Sku", sku.Trim())
            .Set("Variants.$.Price", price)
            .Set("Variants.$.DiscountInformations", discountInformations?.ToDocument())
            .Set("Variants.$.Status", status)
            .Set("Variants.$.Attributes", attributes?.Select(x => x.ToDocument()).ToList())
            .Set("Variants.$.LastUpdatedBy", updatedBy)
            .Set("Variants.$.LastUpdatedAt", updatedAt)
            .Set(x => x.LastUpdatedBy, updatedBy)
            .Set(x => x.LastUpdatedAt, updatedAt);
        var options = new FindOneAndUpdateOptions<ProductDocument>
        {
            ReturnDocument = ReturnDocument.After,
        };

        var document = await _collection.FindOneAndUpdateAsync(GetRequiredSession(), filter, update, options, cancellationToken);
        return document?.ToDomain();
    }

    private IClientSessionHandle GetRequiredSession()
    {
        return _sessionAccessor.Session ?? throw new InvalidOperationException("An active MongoDB session is required for product write operations.");
    }

    private static FilterDefinition<ProductDocument> BuildActiveProductFilter(Guid productId)
    {
        return Builders<ProductDocument>.Filter.Eq(x => x.Id, productId) &
            Builders<ProductDocument>.Filter.Eq(x => x.DeletedAt, null as DateTime?);
    }

    private static FilterDefinition<ProductDocument> BuildActiveProductWithVariantFilter(Guid productId, Guid variantId)
    {
        return BuildActiveProductFilter(productId) &
            Builders<ProductDocument>.Filter.ElemMatch(
                x => x.Variants,
                variant => variant.Id == variantId && variant.DeletedAt == null);
    }

    private static UpdateDefinition<ProductDocument> BuildAppendVariantUpdate(
        VariantDocument variantDocument,
        string? updatedBy,
        DateTime updatedAt)
    {
        var stage = new BsonDocument("$set", new BsonDocument
        {
            {
                "Variants",
                new BsonDocument("$concatArrays", new BsonArray
                {
                    new BsonDocument("$ifNull", new BsonArray
                    {
                        "$Variants",
                        new BsonArray(),
                    }),
                    new BsonArray
                    {
                        variantDocument.ToBsonDocument(),
                    },
                })
            },
            { "LastUpdatedBy", updatedBy },
            { "LastUpdatedAt", updatedAt },
        });

        var pipeline = new EmptyPipelineDefinition<ProductDocument>()
            .AppendStage<ProductDocument, ProductDocument, ProductDocument>(stage);

        return Builders<ProductDocument>.Update.Pipeline(pipeline);
    }

    private static FilterDefinition<ProductDocument> BuildSearchFilter(string? searchText, bool includeDeleted)
    {
        var filter = includeDeleted
            ? Builders<ProductDocument>.Filter.Empty
            : Builders<ProductDocument>.Filter.Eq(x => x.DeletedAt, null as DateTime?);

        var normalizedSearchText = Product.NormalizeSearchText(searchText);
        if (normalizedSearchText is not null)
        {
            filter &= Builders<ProductDocument>.Filter.Regex(
                x => x.NormalizedName,
                new MongoDB.Bson.BsonRegularExpression(Regex.Escape(normalizedSearchText)));
        }

        return filter;
    }
}
