using Celeste.Inventory.Infrastructure.Documents;
using Celeste.Inventory.Infrastructure.Mapping;
using Xunit;

namespace Celeste.Inventory.Infrastructure.Tests.Mapping;

/// <summary>
///     Tests manufacturer infrastructure mapping behavior.
/// </summary>
public sealed class ManufacturerMappingExtensionsTests
{
    [Fact]
    public void ToDomain_WhenDocumentIsActive_MapsFieldsToManufacturer()
    {
        var createdAt = new DateTime(2026, 4, 3, 12, 0, 0, DateTimeKind.Utc);
        var document = new ManufacturerDocument
        {
            Id = Guid.NewGuid(),
            Name = "Celeste Labs",
            ContactEmail = "contact@celeste.test",
            ContactPhone = "4165551234",
            CreatedBy = "alice",
            CreatedAt = createdAt,
        };

        var manufacturer = document.ToDomain();

        Assert.Equal(document.Id, manufacturer.Id);
        Assert.Equal("Celeste Labs", manufacturer.Name);
        Assert.Equal("contact@celeste.test", manufacturer.ContactEmail);
        Assert.Equal("4165551234", manufacturer.ContactPhone);
        Assert.Equal("alice", manufacturer.CreatedBy);
        Assert.Equal(createdAt, manufacturer.CreatedAt);
        Assert.False(manufacturer.IsDeleted);
    }

    [Fact]
    public void ToDomain_WhenDocumentIsDeleted_MapsDeleteAuditFields()
    {
        var deletedAt = new DateTime(2026, 4, 5, 10, 0, 0, DateTimeKind.Utc);
        var document = new ManufacturerDocument
        {
            Id = Guid.NewGuid(),
            Name = "Celeste Labs",
            NormalizedName = "CELESTE LABS",
            CreatedBy = "alice",
            CreatedAt = new DateTime(2026, 4, 3, 12, 0, 0, DateTimeKind.Utc),
            DeletedBy = "charlie",
            DeletedAt = deletedAt,
        };

        var manufacturer = document.ToDomain();

        Assert.Equal(document.Id, manufacturer.Id);
        Assert.Equal("Celeste Labs", manufacturer.Name);
        Assert.Equal("alice", manufacturer.CreatedBy);
        Assert.Equal("charlie", manufacturer.DeletedBy);
        Assert.Equal(deletedAt, manufacturer.DeletedAt);
        Assert.True(manufacturer.IsDeleted);
    }
}
