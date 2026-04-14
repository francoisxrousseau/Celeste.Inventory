namespace Celeste.Inventory.Events;

using Avro;
using Avro.Specific;

/// <summary>
///     Represents variant discount details embedded in product integration events.
/// </summary>
public sealed class DiscountInformations : ISpecificRecord
{
    private const string SchemaJson = """
{
  "type": "record",
  "name": "DiscountInformations",
  "namespace": "Celeste.Inventory.Events",
  "fields": [
    { "name": "DiscountPercentage", "type": "bytes", "logicalType": "decimal", "precision": 18, "scale": 2 },
    { "name": "DiscountStartAtUtc", "type": { "type": "long", "logicalType": "timestamp-millis" } },
    { "name": "DiscountEndAtUtc", "type": { "type": "long", "logicalType": "timestamp-millis" } }
  ]
}
""";

    private static readonly Schema ParsedSchema = Schema.Parse(SchemaJson);

    /// <summary>
    ///     Gets the Avro schema for the discount record.
    /// </summary>
    public Schema Schema => ParsedSchema;

    /// <summary>
    ///     Gets or sets the discount percentage.
    /// </summary>
    public AvroDecimal DiscountPercentage { get; set; } = AvroDecimalSerializer.Serialize(0m);

    /// <summary>
    ///     Gets or sets the UTC discount start timestamp.
    /// </summary>
    public DateTime DiscountStartAtUtc { get; set; }

    /// <summary>
    ///     Gets or sets the UTC discount end timestamp.
    /// </summary>
    public DateTime DiscountEndAtUtc { get; set; }

    /// <summary>
    ///     Reads a field value by Avro position.
    /// </summary>
    /// <param name="fieldPos">
    ///     The zero-based field position.
    /// </param>
    /// <returns>
    ///     The field value.
    /// </returns>
    public object? Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => AvroDecimalSerializer.ToBytes(DiscountPercentage),
            1 => DiscountStartAtUtc,
            2 => DiscountEndAtUtc,
            _ => throw new AvroRuntimeException($"Bad index {fieldPos} in {nameof(DiscountInformations)}"),
        };
    }

    /// <summary>
    ///     Writes a field value by Avro position.
    /// </summary>
    /// <param name="fieldPos">
    ///     The zero-based field position.
    /// </param>
    /// <param name="fieldValue">
    ///     The field value to assign.
    /// </param>
    public void Put(int fieldPos, object? fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                DiscountPercentage = fieldValue switch
                {
                    AvroDecimal value => value,
                    decimal value => AvroDecimalSerializer.Serialize(value),
                    byte[] value => AvroDecimalSerializer.FromBytes(value),
                    IReadOnlyList<byte> value => AvroDecimalSerializer.FromBytes(value.ToArray()),
                    _ => DiscountPercentage,
                };
                break;
            case 1:
                DiscountStartAtUtc = fieldValue switch
                {
                    DateTime value => value,
                    long milliseconds => DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime,
                    _ => default,
                };
                break;
            case 2:
                DiscountEndAtUtc = fieldValue switch
                {
                    DateTime value => value,
                    long milliseconds => DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime,
                    _ => default,
                };
                break;
            default:
                throw new AvroRuntimeException($"Bad index {fieldPos} in {nameof(DiscountInformations)}");
        }
    }
}
