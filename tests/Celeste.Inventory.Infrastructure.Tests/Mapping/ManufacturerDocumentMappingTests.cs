namespace Celeste.Inventory.Infrastructure.Tests.Mapping;

using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Infrastructure.Mapping;
using Xunit;

/// <summary>
///     Tests mapping from domain manufacturer entities to Mongo documents.
/// </summary>
public sealed class ManufacturerDocumentMappingTests
{
    [Fact]
    public void ToDocument_WhenManufacturerIsMapped_SetsNormalizedNameAndAuditFields()
    {
        var createdAt = new DateTime(2026, 4, 3, 12, 0, 0, DateTimeKind.Utc);
        var manufacturer = new Manufacturer
        {
            Id = Guid.NewGuid(),
            Name = "  Celeste Labs  ",
            ContactEmail = "contact@celeste.test",
            ContactPhone = "4165551234",
            CreatedBy = "alice",
            CreatedAt = createdAt,
            LastUpdatedBy = "bob",
            LastUpdatedAt = new DateTime(2026, 4, 4, 9, 15, 0, DateTimeKind.Utc),
            DeletedBy = "charlie",
            DeletedAt = new DateTime(2026, 4, 5, 10, 0, 0, DateTimeKind.Utc),
        };

        var document = manufacturer.ToDocument();

        Assert.Equal(manufacturer.Id, document.Id);
        Assert.Equal("  Celeste Labs  ", document.Name);
        Assert.Equal("CELESTE LABS", document.NormalizedName);
        Assert.Equal("contact@celeste.test", document.ContactEmail);
        Assert.Equal("4165551234", document.ContactPhone);
        Assert.Equal("alice", document.CreatedBy);
        Assert.Equal(createdAt, document.CreatedAt);
        Assert.Equal("bob", document.LastUpdatedBy);
        Assert.Equal(manufacturer.LastUpdatedAt, document.LastUpdatedAt);
        Assert.Equal("charlie", document.DeletedBy);
        Assert.Equal(manufacturer.DeletedAt, document.DeletedAt);
    }
}
