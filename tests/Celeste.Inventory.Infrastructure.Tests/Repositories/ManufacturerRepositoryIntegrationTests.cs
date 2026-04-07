namespace Celeste.Inventory.Infrastructure.Tests.Repositories;

using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Infrastructure.Documents;
using Celeste.Inventory.Infrastructure.Repositories;
using Mongo2Go;
using MongoDB.Driver;
using Xunit;

/// <summary>
///     Tests manufacturer repository persistence behavior against MongoDB.
/// </summary>
public sealed class ManufacturerRepositoryIntegrationTests : IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<ManufacturerDocument> _collection;
    private readonly ManufacturerRepository _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ManufacturerRepositoryIntegrationTests"/> class.
    /// </summary>
    public ManufacturerRepositoryIntegrationTests()
    {
        _runner = MongoDbRunner.Start();
        var client = new MongoClient(_runner.ConnectionString);
        _database = client.GetDatabase($"manufacturer-tests-{Guid.NewGuid():N}");
        _collection = _database.GetCollection<ManufacturerDocument>("manufacturers");
        _repository = new ManufacturerRepository(_database);
    }

    [Fact]
    public async Task CreateAsync_ThenGetByIdAsync_PersistsAndReturnsManufacturer()
    {
        var manufacturer = CreateManufacturer("Celeste Labs", "alice", Utc(2026, 4, 1, 8, 0));

        await _repository.CreateAsync(manufacturer, CancellationToken.None);
        var result = await _repository.GetByIdAsync(manufacturer.Id, includeDeleted: false, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(manufacturer.Id, result.Id);
        Assert.Equal("Celeste Labs", result.Name);
        Assert.Equal("CELESTE LABS", (await LoadDocumentAsync(manufacturer.Id)).NormalizedName);
    }

    [Fact]
    public async Task GetByIdAsync_ExcludesDeletedByDefault_AndIncludesWhenRequested()
    {
        var manufacturer = CreateManufacturer("Celeste Labs", "alice", Utc(2026, 4, 1, 8, 0));
        await _repository.CreateAsync(manufacturer, CancellationToken.None);
        await _repository.DeleteAsync(manufacturer.Id, "bob", Utc(2026, 4, 2, 9, 30), CancellationToken.None);

        var excluded = await _repository.GetByIdAsync(manufacturer.Id, includeDeleted: false, CancellationToken.None);
        var included = await _repository.GetByIdAsync(manufacturer.Id, includeDeleted: true, CancellationToken.None);

        Assert.Null(excluded);
        Assert.NotNull(included);
        Assert.True(included.IsDeleted);
        Assert.Equal("bob", included.DeletedBy);
    }

    [Fact]
    public async Task SearchAsync_ReturnsPagedNormalizedMatches_AndRespectsIncludeDeleted()
    {
        await SeedAsync(
            CreateManufacturer("Celeste Labs", "alice", Utc(2026, 4, 1, 8, 0)),
            CreateManufacturer("Celestial Works", "alice", Utc(2026, 4, 1, 8, 1)),
            CreateManufacturer("Northern Supply", "alice", Utc(2026, 4, 1, 8, 2)));

        var deleted = CreateManufacturer("Celeste Archived", "alice", Utc(2026, 4, 1, 8, 3));
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
            CreateManufacturer("Celeste Labs", "alice", Utc(2026, 4, 1, 8, 0)),
            CreateManufacturer("Celestial Works", "alice", Utc(2026, 4, 1, 8, 1)));

        var deleted = CreateManufacturer("Celeste Deleted", "alice", Utc(2026, 4, 1, 8, 2));
        await _repository.CreateAsync(deleted, CancellationToken.None);
        await _repository.DeleteAsync(deleted.Id, "bob", Utc(2026, 4, 2, 9, 30), CancellationToken.None);

        var activeCount = await _repository.CountAsync("celes", includeDeleted: false, CancellationToken.None);
        var allCount = await _repository.CountAsync("celes", includeDeleted: true, CancellationToken.None);

        Assert.Equal(2, activeCount);
        Assert.Equal(3, allCount);
    }

    [Fact]
    public async Task ExistsByNameAsync_IsCaseInsensitive_AndIgnoresDeletedAndExcludedId()
    {
        var active = CreateManufacturer("Celeste Labs", "alice", Utc(2026, 4, 1, 8, 0));
        var deleted = CreateManufacturer("Deleted Name", "alice", Utc(2026, 4, 1, 8, 1));

        await _repository.CreateAsync(active, CancellationToken.None);
        await _repository.CreateAsync(deleted, CancellationToken.None);
        await _repository.DeleteAsync(deleted.Id, "bob", Utc(2026, 4, 2, 9, 30), CancellationToken.None);

        Assert.True(await _repository.ExistsByNameAsync("  celeste labs  ", cancellationToken: CancellationToken.None));
        Assert.False(await _repository.ExistsByNameAsync("deleted name", cancellationToken: CancellationToken.None));
        Assert.False(await _repository.ExistsByNameAsync("Celeste Labs", excludedId: active.Id, cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletes_AndSecondDeleteReturnsFalse()
    {
        var manufacturer = CreateManufacturer("Celeste Labs", "alice", Utc(2026, 4, 1, 8, 0));
        await _repository.CreateAsync(manufacturer, CancellationToken.None);

        var first = await _repository.DeleteAsync(manufacturer.Id, "bob", Utc(2026, 4, 2, 9, 30), CancellationToken.None);
        var second = await _repository.DeleteAsync(manufacturer.Id, "charlie", Utc(2026, 4, 3, 10, 15), CancellationToken.None);
        var document = await LoadDocumentAsync(manufacturer.Id);

        Assert.True(first);
        Assert.False(second);
        Assert.Equal("bob", document.DeletedBy);
        Assert.Equal(Utc(2026, 4, 2, 9, 30), document.DeletedAt);
    }

    [Fact]
    public async Task UpdateAsync_WithEntity_PreservesCreationAuditFields()
    {
        var manufacturer = CreateManufacturer("Celeste Labs", "alice", Utc(2026, 4, 1, 8, 0));
        await _repository.CreateAsync(manufacturer, CancellationToken.None);

        manufacturer.Name = "Celeste Research";
        manufacturer.ContactEmail = "research@celeste.test";
        manufacturer.LastUpdatedBy = "bob";
        manufacturer.LastUpdatedAt = Utc(2026, 4, 2, 9, 30);

        await _repository.UpdateAsync(manufacturer, CancellationToken.None);
        var document = await LoadDocumentAsync(manufacturer.Id);

        Assert.Equal("Celeste Research", document.Name);
        Assert.Equal("CELESTE RESEARCH", document.NormalizedName);
        Assert.Equal("alice", document.CreatedBy);
        Assert.Equal(Utc(2026, 4, 1, 8, 0), document.CreatedAt);
        Assert.Equal("bob", document.LastUpdatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithFields_UpdatesActiveManufacturerOnly()
    {
        var active = CreateManufacturer("Celeste Labs", "alice", Utc(2026, 4, 1, 8, 0));
        await _repository.CreateAsync(active, CancellationToken.None);

        var updated = await _repository.UpdateAsync(
            active.Id,
            "  Celeste Research  ",
            "research@celeste.test",
            "6475559999",
            "bob",
            Utc(2026, 4, 2, 9, 30),
            CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("Celeste Research", updated.Name);
        Assert.Equal("research@celeste.test", updated.ContactEmail);
        Assert.Equal("bob", updated.LastUpdatedBy);
        Assert.Equal(Utc(2026, 4, 2, 9, 30), updated.LastUpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_WithFields_ReturnsNullForDeletedManufacturer()
    {
        var deleted = CreateManufacturer("Celeste Labs", "alice", Utc(2026, 4, 1, 8, 0));
        await _repository.CreateAsync(deleted, CancellationToken.None);
        await _repository.DeleteAsync(deleted.Id, "bob", Utc(2026, 4, 2, 9, 30), CancellationToken.None);

        var updated = await _repository.UpdateAsync(
            deleted.Id,
            "Celeste Research",
            "research@celeste.test",
            "6475559999",
            "charlie",
            Utc(2026, 4, 3, 10, 15),
            CancellationToken.None);

        Assert.Null(updated);
    }

    public void Dispose()
    {
        _runner.Dispose();
    }

    private async Task SeedAsync(params Manufacturer[] manufacturers)
    {
        foreach (var manufacturer in manufacturers)
        {
            await _repository.CreateAsync(manufacturer, CancellationToken.None);
        }
    }

    private async Task<ManufacturerDocument> LoadDocumentAsync(Guid id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(CancellationToken.None)
            ?? throw new InvalidOperationException("Expected document was not found.");
    }

    private static Manufacturer CreateManufacturer(string name, string? createdBy, DateTime createdAt)
    {
        return new Manufacturer
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedBy = createdBy,
            CreatedAt = createdAt,
        };
    }

    private static DateTime Utc(int year, int month, int day, int hour, int minute)
    {
        return new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
    }
}
