namespace Celeste.Inventory.Infrastructure.Repositories;

using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Core.Repositories;
using Celeste.Inventory.Infrastructure.Documents;
using Celeste.Inventory.Infrastructure.Mapping;
using Emit.MongoDB;
using MongoDB.Driver;
using System.Text.RegularExpressions;

/// <summary>
///     Provides MongoDB persistence for manufacturers.
/// </summary>
public sealed class ManufacturerRepository : IManufacturerRepository
{
    private readonly IMongoCollection<ManufacturerDocument> _collection;
    private readonly IMongoSessionAccessor _sessionAccessor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ManufacturerRepository"/> class.
    /// </summary>
    /// <param name="database">
    ///     The Mongo database used for manufacturer persistence.
    /// </param>
    /// <param name="sessionAccessor">
    ///     Provides access to the active MongoDB session.
    /// </param>
    public ManufacturerRepository(
        IMongoDatabase database,
        IMongoSessionAccessor sessionAccessor)
    {
        _collection = database.GetCollection<ManufacturerDocument>("manufacturers");
        _sessionAccessor = sessionAccessor;
    }

    /// <inheritdoc />
    public Task<long> CountAsync(string? searchText, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        return _collection.CountDocumentsAsync(BuildSearchFilter(searchText, includeDeleted), cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task CreateAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default)
    {
        var document = manufacturer.ToDocument();
        return _collection.InsertOneAsync(GetRequiredSession(), document, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Manufacturer?> DeleteAsync(Guid id, string? deletedBy, DateTime deletedAt, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ManufacturerDocument>.Filter.Eq(x => x.Id, id) &
            Builders<ManufacturerDocument>.Filter.Eq(x => x.DeletedAt, null as DateTime?);
        var update = Builders<ManufacturerDocument>.Update
            .Set(x => x.DeletedBy, deletedBy)
            .Set(x => x.DeletedAt, deletedAt);
        var options = new FindOneAndUpdateOptions<ManufacturerDocument>
        {
            ReturnDocument = ReturnDocument.After,
        };

        var document = await _collection.FindOneAndUpdateAsync(GetRequiredSession(), filter, update, options, cancellationToken);
        return document?.ToDomain();
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(string name, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var normalizedName = Manufacturer.NormalizeSearchText(name);
        if (normalizedName is null)
        {
            return false;
        }

        var filter = Builders<ManufacturerDocument>.Filter.Eq(x => x.NormalizedName, normalizedName) &
            Builders<ManufacturerDocument>.Filter.Eq(x => x.DeletedAt, null as DateTime?);

        if (excludedId.HasValue)
        {
            filter &= Builders<ManufacturerDocument>.Filter.Ne(x => x.Id, excludedId.Value);
        }

        var count = await _collection.CountDocumentsAsync(filter, new CountOptions { Limit = 1 }, cancellationToken);
        return count > 0;
    }

    /// <inheritdoc />
    public async Task<Manufacturer?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ManufacturerDocument>.Filter.Eq(x => x.Id, id);
        if (!includeDeleted)
        {
            filter &= Builders<ManufacturerDocument>.Filter.Eq(x => x.DeletedAt, null as DateTime?);
        }

        var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (document is null)
        {
            return null;
        }

        return document.ToDomain();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Manufacturer>> SearchAsync(string? searchText, int pageNumber, int pageSize, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var documents = await _collection.Find(BuildSearchFilter(searchText, includeDeleted))
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return documents.Select(x => x.ToDomain()).ToList();
    }

    /// <inheritdoc />
    public Task UpdateAsync(Manufacturer manufacturer, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ManufacturerDocument>.Filter.Eq(x => x.Id, manufacturer.Id);
        var update = Builders<ManufacturerDocument>.Update
            .Set(x => x.Name, manufacturer.Name)
            .Set(x => x.NormalizedName, Manufacturer.NormalizeSearchText(manufacturer.Name) ?? string.Empty)
            .Set(x => x.ContactEmail, manufacturer.ContactEmail)
            .Set(x => x.ContactPhone, manufacturer.ContactPhone)
            .Set(x => x.LastUpdatedBy, manufacturer.LastUpdatedBy)
            .Set(x => x.LastUpdatedAt, manufacturer.LastUpdatedAt)
            .Set(x => x.DeletedBy, manufacturer.DeletedBy)
            .Set(x => x.DeletedAt, manufacturer.DeletedAt);

        return _collection.UpdateOneAsync(GetRequiredSession(), filter, update, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Manufacturer?> UpdateAsync(Guid id, string name, string? contactEmail, string? contactPhone, string? updatedBy, DateTime updatedAt, CancellationToken cancellationToken = default)
    {
        var trimmedName = name.Trim();
        var normalizedName = Manufacturer.NormalizeSearchText(name) ?? string.Empty;
        var filter = Builders<ManufacturerDocument>.Filter.Eq(x => x.Id, id) &
            Builders<ManufacturerDocument>.Filter.Eq(x => x.DeletedAt, null as DateTime?);
        var update = Builders<ManufacturerDocument>.Update
            .Set(x => x.Name, trimmedName)
            .Set(x => x.NormalizedName, normalizedName)
            .Set(x => x.ContactEmail, contactEmail)
            .Set(x => x.ContactPhone, contactPhone)
            .Set(x => x.LastUpdatedBy, updatedBy)
            .Set(x => x.LastUpdatedAt, updatedAt);

        var options = new FindOneAndUpdateOptions<ManufacturerDocument>
        {
            ReturnDocument = ReturnDocument.After,
        };
        var document = await _collection.FindOneAndUpdateAsync(GetRequiredSession(), filter, update, options, cancellationToken);

        return document?.ToDomain();
    }

    private IClientSessionHandle GetRequiredSession()
    {
        return _sessionAccessor.Session ?? throw new InvalidOperationException("An active MongoDB session is required for manufacturer write operations.");
    }

    private static FilterDefinition<ManufacturerDocument> BuildSearchFilter(string? searchText, bool includeDeleted)
    {
        var filter = includeDeleted
            ? Builders<ManufacturerDocument>.Filter.Empty
            : Builders<ManufacturerDocument>.Filter.Eq(x => x.DeletedAt, null as DateTime?);

        var normalizedSearchText = Manufacturer.NormalizeSearchText(searchText);
        if (normalizedSearchText is not null)
        {
            filter &= Builders<ManufacturerDocument>.Filter.Regex(
                x => x.NormalizedName,
                new MongoDB.Bson.BsonRegularExpression(Regex.Escape(normalizedSearchText)));
        }

        return filter;
    }
}
