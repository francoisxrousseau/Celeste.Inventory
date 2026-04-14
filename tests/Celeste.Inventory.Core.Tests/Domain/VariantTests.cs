namespace Celeste.Inventory.Core.Tests.Domain;

using Celeste.Inventory.Common.Enums;
using Celeste.Inventory.Core.Domain;
using Xunit;

/// <summary>
///     Tests the observable variant domain shape.
/// </summary>
public sealed class VariantTests
{
    [Fact]
    public void Product_WithVariant_PreservesVariantShapeAndAuditFields()
    {
        var createdAt = new DateTime(2026, 4, 6, 10, 30, 0, DateTimeKind.Utc);
        var discountStartAtUtc = new DateTime(2026, 4, 7, 0, 0, 0, DateTimeKind.Utc);
        var discountEndAtUtc = new DateTime(2026, 4, 14, 0, 0, 0, DateTimeKind.Utc);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            ManufacturerId = Guid.NewGuid(),
            Name = "Celeste Tee",
            Variants =
            [
                new Variant
                {
                    Id = Guid.NewGuid(),
                    Sku = "TEE-RED-M",
                    Price = 24.99m,
                    DiscountInformations = new DiscountInformations
                    {
                        DiscountPercentage = 15,
                        DiscountStartAtUtc = discountStartAtUtc,
                        DiscountEndAtUtc = discountEndAtUtc,
                    },
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
                    CreatedBy = "alice",
                    CreatedAt = createdAt,
                },
            ],
        };

        var variant = Assert.Single(product.Variants);

        Assert.NotEqual(Guid.Empty, variant.Id);
        Assert.Equal("TEE-RED-M", variant.Sku);
        Assert.Equal(24.99m, variant.Price);
        Assert.NotNull(variant.DiscountInformations);
        Assert.Equal(15, variant.DiscountInformations!.DiscountPercentage);
        Assert.Equal(discountStartAtUtc, variant.DiscountInformations.DiscountStartAtUtc);
        Assert.Equal(discountEndAtUtc, variant.DiscountInformations.DiscountEndAtUtc);
        Assert.Equal(ProductStatus.Active, variant.Status);
        Assert.Equal(2, variant.Attributes?.Count);
        Assert.Equal("Color", variant.Attributes![0].Name);
        Assert.Equal("Red", variant.Attributes[0].Value);
        Assert.Equal("Size", variant.Attributes[1].Name);
        Assert.Equal("M", variant.Attributes[1].Value);
        Assert.Equal("alice", variant.CreatedBy);
        Assert.Equal(createdAt, variant.CreatedAt);
        Assert.Null(variant.LastUpdatedBy);
        Assert.Null(variant.LastUpdatedAt);
        Assert.Null(variant.DeletedBy);
        Assert.Null(variant.DeletedAt);
        Assert.False(variant.IsDeleted);
    }
}
