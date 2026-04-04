using Celeste.Inventory.Core.Domain;
using Xunit;

namespace Celeste.Inventory.Core.Tests.Domain;

/// <summary>
///     Tests the observable manufacturer domain shape.
/// </summary>
public sealed class ManufacturerTests
{
    [Fact]
    public void ObjectInitializer_WithValues_PreservesAssignedState()
    {
        var createdAt = new DateTime(2026, 4, 3, 12, 0, 0, DateTimeKind.Utc);

        var manufacturer = new Manufacturer
        {
            Id = Guid.NewGuid(),
            Name = "Celeste Labs",
            ContactEmail = "contact@celeste.test",
            ContactPhone = "4165551234",
            CreatedBy = "alice",
            CreatedAt = createdAt,
        };

        Assert.NotEqual(Guid.Empty, manufacturer.Id);
        Assert.Equal("Celeste Labs", manufacturer.Name);
        Assert.Equal("contact@celeste.test", manufacturer.ContactEmail);
        Assert.Equal("4165551234", manufacturer.ContactPhone);
        Assert.Equal("alice", manufacturer.CreatedBy);
        Assert.Equal(createdAt, manufacturer.CreatedAt);
        Assert.Null(manufacturer.LastUpdatedBy);
        Assert.Null(manufacturer.LastUpdatedAt);
        Assert.Null(manufacturer.DeletedBy);
        Assert.Null(manufacturer.DeletedAt);
        Assert.False(manufacturer.IsDeleted);
    }

    [Fact]
    public void IsDeleted_WhenDeletedAtIsNull_ReturnsFalse()
    {
        var manufacturer = new Manufacturer();

        Assert.False(manufacturer.IsDeleted);
    }

    [Fact]
    public void IsDeleted_WhenDeletedAtHasValue_ReturnsTrue()
    {
        var deletedAt = new DateTime(2026, 4, 5, 8, 45, 0, DateTimeKind.Utc);
        var manufacturer = new Manufacturer
        {
            DeletedAt = deletedAt,
            DeletedBy = "charlie",
        };

        Assert.True(manufacturer.IsDeleted);
        Assert.Equal(deletedAt, manufacturer.DeletedAt);
        Assert.Equal("charlie", manufacturer.DeletedBy);
    }
}
