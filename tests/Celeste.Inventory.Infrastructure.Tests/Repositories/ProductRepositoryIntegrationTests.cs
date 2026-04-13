namespace Celeste.Inventory.Infrastructure.Tests.Repositories;

using Celeste.Inventory.Common.Enums;
using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Infrastructure.Documents;
using Celeste.Inventory.Infrastructure.Repositories;
using Emit.MongoDB;
using Mongo2Go;
using MongoDB.Driver;
using Xunit;

/// <summary>
///     Tests product repository persistence behavior against MongoDB.
/// </summary>
public sealed class ProductRepositoryIntegrationTests : IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<ProductDocument> _collection;
    private readonly IClientSessionHandle _session;
    private readonly ProductRepository _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProductRepositoryIntegrationTests"/> class.
    /// </summary>
    public ProductRepositoryIntegrationTests()
    {
        _runner = MongoDbRunner.Start();
        var client = new MongoClient(_runner.ConnectionString);
        _database = client.GetDatabase($"product-tests-{Guid.NewGuid():N}");
        _collection = _database.GetCollection<ProductDocument>("products");
        _session = client.StartSession();
        _repository = new ProductRepository(_database, new FakeMongoSessionAccessor(_session));
    }

    [Fact]
    public async Task CreateAsync_ThenGetByIdAsync_PersistsAndReturnsProduct()
    {
        var product = CreateProduct("Celeste Tee", "alice", Utc(2026, 4, 1, 8, 0));

        await _repository.CreateAsync(product, CancellationToken.None);
        var result = await _repository.GetByIdAsync(product.Id, includeDeleted: false, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal("Celeste Tee", result.Name);
        Assert.Equal(ProductStatus.Active, result.Status);
        Assert.Equal("CELESTE TEE", (await LoadDocumentAsync(product.Id)).NormalizedName);
    }

    [Fact]
    public async Task AddVariantAsync_ThenGetVariantByIdAsync_PersistsAndReturnsVariant()
    {
        var product = CreateProduct("Celeste Tee", "alice", Utc(2026, 4, 1, 8, 0));
        await _repository.CreateAsync(product, CancellationToken.None);

        var variant = CreateVariant("TEE-RED-M", "alice", Utc(2026, 4, 6, 10, 30));

        var created = await _repository.AddVariantAsync(
            product.Id,
            variant,
            "alice",
            Utc(2026, 4, 6, 10, 30),
            CancellationToken.None);

        var result = await _repository.GetVariantByIdAsync(product.Id, variant.Id, includeDeleted: false, CancellationToken.None);

        Assert.NotNull(created);
        Assert.Equal(product.Id, created!.Id);
        Assert.NotNull(created.Variants);
        Assert.Single(created.Variants!);

        Assert.NotNull(result);
        Assert.Equal(variant.Id, result!.Id);
        Assert.Equal("TEE-RED-M", result.Sku);
        Assert.Equal(24.99m, result.Price);
        Assert.Equal("alice", result.CreatedBy);
        Assert.Equal(Utc(2026, 4, 6, 10, 30), result.CreatedAt);
        Assert.False(result.IsDeleted);
    }

    [Fact]
    public async Task CreateAsync_WithoutActiveSession_ThrowsInvalidOperationException()
    {
        var repository = new ProductRepository(_database, new FakeMongoSessionAccessor(null));
        var product = CreateProduct("Celeste Tee", "alice", Utc(2026, 4, 1, 8, 0));

        var action = () => repository.CreateAsync(product, CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    [Fact]
    public async Task SearchAsync_ReturnsPagedNormalizedMatches_AndRespectsIncludeDeleted()
    {
        await SeedAsync(
            CreateProduct("Celeste Tee", "alice", Utc(2026, 4, 1, 8, 0)),
            CreateProduct("Celestial Cap", "alice", Utc(2026, 4, 1, 8, 1)),
            CreateProduct("Northern Boots", "alice", Utc(2026, 4, 1, 8, 2)));

        var deleted = CreateProduct("Celeste Archived", "alice", Utc(2026, 4, 1, 8, 3));
        await _repository.CreateAsync(deleted, CancellationToken.None);
        await _repository.DeleteAsync(deleted.Id, "bob", Utc(2026, 4, 2, 9, 30), CancellationToken.None);

        var pageOne = await _repository.SearchAsync(" celes ", 1, 2, includeDeleted: false, CancellationToken.None);
        var withDeleted = await _repository.SearchAsync("celes", 1, 10, includeDeleted: true, CancellationToken.None);

        Assert.Equal(2, pageOne.Count);
        Assert.DoesNotContain(pageOne, x => x.Name == "Celeste Archived");
        Assert.Contains(withDeleted, x => x.Name == "Celeste Archived");
    }

    [Fact]
    public async Task CountAsync_ReturnsMatchingTotals_WithAndWithoutDeleted()
    {
        await SeedAsync(
            CreateProduct("Celeste Tee", "alice", Utc(2026, 4, 1, 8, 0)),
            CreateProduct("Celestial Cap", "alice", Utc(2026, 4, 1, 8, 1)));

        var deleted = CreateProduct("Celeste Deleted", "alice", Utc(2026, 4, 1, 8, 2));
        await _repository.CreateAsync(deleted, CancellationToken.None);
        await _repository.DeleteAsync(deleted.Id, "bob", Utc(2026, 4, 2, 9, 30), CancellationToken.None);

        var activeCount = await _repository.CountAsync("celes", includeDeleted: false, CancellationToken.None);
        var allCount = await _repository.CountAsync("celes", includeDeleted: true, CancellationToken.None);

        Assert.Equal(2, activeCount);
        Assert.Equal(3, allCount);
    }

    [Fact]
    public async Task UpdateAsync_WithFields_UpdatesActiveProductOnly()
    {
        var active = CreateProduct("Celeste Tee", "alice", Utc(2026, 4, 1, 8, 0));
        await _repository.CreateAsync(active, CancellationToken.None);

        var updated = await _repository.UpdateAsync(
            active.Id,
            active.ManufacturerId,
            "  Celeste Shirt  ",
            "  cotton  ",
            ProductStatus.Inactive,
            ProductCategory.Apparel,
            ["shirt", "cotton"],
            "bob",
            Utc(2026, 4, 2, 9, 30),
            CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("Celeste Shirt", updated.Name);
        Assert.Equal("  cotton  ", updated.Description);
        Assert.Equal(ProductStatus.Inactive, updated.Status);
        Assert.Equal(ProductCategory.Apparel, updated.Category);
        Assert.Equal("bob", updated.LastUpdatedBy);
        Assert.Equal(Utc(2026, 4, 2, 9, 30), updated.LastUpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_WithFields_ReturnsNullForDeletedProduct()
    {
        var deleted = CreateProduct("Celeste Tee", "alice", Utc(2026, 4, 1, 8, 0));
        await _repository.CreateAsync(deleted, CancellationToken.None);
        await _repository.DeleteAsync(deleted.Id, "bob", Utc(2026, 4, 2, 9, 30), CancellationToken.None);

        var updated = await _repository.UpdateAsync(
            deleted.Id,
            deleted.ManufacturerId,
            "Celeste Shirt",
            "cotton",
            ProductStatus.Inactive,
            ProductCategory.Apparel,
            ["shirt"],
            "charlie",
            Utc(2026, 4, 3, 10, 15),
            CancellationToken.None);

        Assert.Null(updated);
    }

    [Fact]
    public async Task UpdateAsync_WithEntity_WithoutActiveSession_ThrowsInvalidOperationException()
    {
        var repository = new ProductRepository(_database, new FakeMongoSessionAccessor(null));
        var product = CreateProduct("Celeste Tee", "alice", Utc(2026, 4, 1, 8, 0));

        var action = () => repository.UpdateAsync(product, CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletes_AndSecondDeleteReturnsNull()
    {
        var product = CreateProduct("Celeste Tee", "alice", Utc(2026, 4, 1, 8, 0));
        await _repository.CreateAsync(product, CancellationToken.None);

        var first = await _repository.DeleteAsync(product.Id, "bob", Utc(2026, 4, 2, 9, 30), CancellationToken.None);
        var second = await _repository.DeleteAsync(product.Id, "charlie", Utc(2026, 4, 3, 10, 15), CancellationToken.None);
        var document = await LoadDocumentAsync(product.Id);

        Assert.NotNull(first);
        Assert.Null(second);
        Assert.True(first.IsDeleted);
        Assert.Equal("bob", document.DeletedBy);
        Assert.Equal(Utc(2026, 4, 2, 9, 30), document.DeletedAt);
    }

    [Fact]
    public async Task DeleteAsync_WithoutActiveSession_ThrowsInvalidOperationException()
    {
        var repository = new ProductRepository(_database, new FakeMongoSessionAccessor(null));

        var action = () => repository.DeleteAsync(Guid.NewGuid(), "bob", Utc(2026, 4, 2, 9, 30), CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    [Fact]
    public async Task UpdateAsync_WithFields_WithoutActiveSession_ThrowsInvalidOperationException()
    {
        var repository = new ProductRepository(_database, new FakeMongoSessionAccessor(null));

        var action = () => repository.UpdateAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Celeste Shirt",
            "cotton",
            ProductStatus.Active,
            ProductCategory.Apparel,
            null,
            "bob",
            Utc(2026, 4, 2, 9, 30),
            CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    public void Dispose()
    {
        _session.Dispose();
        _runner.Dispose();
    }

    private async Task SeedAsync(params Product[] products)
    {
        foreach (var product in products)
        {
            await _repository.CreateAsync(product, CancellationToken.None);
        }
    }

    private async Task<ProductDocument> LoadDocumentAsync(Guid id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(CancellationToken.None)
            ?? throw new InvalidOperationException("Expected document was not found.");
    }

    private static Product CreateProduct(string name, string? createdBy, DateTime createdAt)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            ManufacturerId = Guid.NewGuid(),
            Name = name,
            Description = "desc",
            Status = ProductStatus.Active,
            Category = ProductCategory.Apparel,
            Tags = ["tag-1"],
            Variants = [],
            CreatedBy = createdBy,
            CreatedAt = createdAt,
        };
    }

    private static Variant CreateVariant(string sku, string? createdBy, DateTime createdAt)
    {
        return new Variant
        {
            Id = Guid.NewGuid(),
            Sku = sku,
            Price = 24.99m,
            Status = ProductStatus.Active,
            Attributes =
            [
                new VariantAttribute
                {
                    Name = "Color",
                    Value = "Red",
                },
                new VariantAttribute
                {
                    Name = "Size",
                    Value = "M",
                },
            ],
            CreatedBy = createdBy,
            CreatedAt = createdAt,
        };
    }

    private static DateTime Utc(int year, int month, int day, int hour, int minute)
    {
        return new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
    }

    private sealed class FakeMongoSessionAccessor(IClientSessionHandle? session) : IMongoSessionAccessor
    {
        public IClientSessionHandle? Session { get; } = session;
    }
}
