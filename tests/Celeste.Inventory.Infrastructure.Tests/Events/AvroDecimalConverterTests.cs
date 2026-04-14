namespace Celeste.Inventory.Infrastructure.Tests.Events;

using Avro;
using Avro.IO;
using Avro.Specific;
using Celeste.Inventory.Events;
using Xunit;

/// <summary>
///     Tests Avro decimal serialization behavior for event records.
/// </summary>
public sealed class AvroDecimalSerializerTests
{
    [Fact]
    public void Serialize_AndDeserialize_RoundTripDecimalValues()
    {
        var value = 24.99m;

        var avroDecimal = AvroDecimalSerializer.Serialize(value);
        var result = AvroDecimalSerializer.Deserialize(avroDecimal);

        Assert.Equal(2, avroDecimal.Scale);
        Assert.Equal(value, result);
    }

    [Fact]
    public void Variant_GetAndPut_UsesAvroDecimalInternally()
    {
        var avroDecimal = AvroDecimalSerializer.Serialize(19.95m);
        var variant = new Variant
        {
            Price = avroDecimal,
        };

        var serialized = variant.Get(2);
        variant.Put(2, avroDecimal);

        Assert.IsType<AvroDecimal>(serialized);
        Assert.Equal(avroDecimal, serialized);
        Assert.Equal(19.95m, AvroDecimalSerializer.Deserialize(variant.Price.Value));
    }

    [Fact]
    public void DiscountInformations_GetAndPut_UsesAvroDecimalInternally()
    {
        var avroDecimal = AvroDecimalSerializer.Serialize(12.5m);
        var bytes = AvroDecimalSerializer.ToBytes(avroDecimal);
        var discount = new DiscountInformations
        {
            DiscountPercentage = avroDecimal,
        };

        var serialized = discount.Get(0);
        discount.Put(0, bytes);

        Assert.IsType<byte[]>(serialized);
        Assert.Equal(bytes, (byte[])serialized!);
        Assert.Equal(12.5m, AvroDecimalSerializer.Deserialize(discount.DiscountPercentage));
    }

    [Fact]
    public void ProductEvent_WithVariantDecimals_CanBeWrittenByApacheAvro()
    {
        var productEvent = ProductEventFactory.VariantCreated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Celeste Tee",
            "A cotton tee",
            "Active",
            "Apparel",
            ["summer"],
            new Variant
            {
                Id = Guid.NewGuid(),
                Sku = "TEE-RED-M",
                Price = AvroDecimalSerializer.Serialize(24.99m),
                Discount = new DiscountInformations
                {
                    DiscountPercentage = AvroDecimalSerializer.Serialize(15m),
                    DiscountStartAtUtc = new DateTime(2026, 4, 13, 12, 0, 0, DateTimeKind.Utc),
                    DiscountEndAtUtc = new DateTime(2026, 4, 20, 12, 0, 0, DateTimeKind.Utc),
                },
                Status = "Active",
            },
            "alice",
            new DateTime(2026, 4, 13, 12, 0, 0, DateTimeKind.Utc));

        using var stream = new MemoryStream();
        var writer = new SpecificDefaultWriter(productEvent.Schema);
        var encoder = new BinaryEncoder(stream);

        writer.Write(productEvent, encoder);

        Assert.True(stream.Length > 0);
    }
}
