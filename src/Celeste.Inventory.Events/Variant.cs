namespace Celeste.Inventory.Events;

using Avro;
using Avro.Specific;

/// <summary>
///     Represents variant details embedded in product integration events.
/// </summary>
public sealed class Variant : ISpecificRecord
{
    internal const string SchemaJson = """
{
  "type": "record",
  "name": "Variant",
  "namespace": "Celeste.Inventory.Events",
  "fields": [
    { "name": "Id", "type": { "type": "string", "logicalType": "uuid" } },
    { "name": "Sku", "type": [ "null", "string" ], "default": null },
    { "name": "Price", "type": [ "null", { "type": "bytes", "logicalType": "decimal", "precision": 18, "scale": 2 } ], "default": null },
    { "name": "Discount", "type": [ "null", {
      "type": "record",
      "name": "DiscountInformations",
      "fields": [
        { "name": "DiscountPercentage", "type": "bytes", "logicalType": "decimal", "precision": 18, "scale": 2 },
        { "name": "DiscountStartAtUtc", "type": { "type": "long", "logicalType": "timestamp-millis" } },
        { "name": "DiscountEndAtUtc", "type": { "type": "long", "logicalType": "timestamp-millis" } }
      ]
    }], "default": null },
    { "name": "Status", "type": [ "null", "string" ], "default": null },
    { "name": "Attributes", "type": [ "null", {
      "type": "array",
      "items": {
        "type": "record",
        "name": "VariantAttribute",
        "fields": [
          { "name": "Name", "type": "string" },
          { "name": "Value", "type": "string" }
        ]
      }
    }], "default": null }
  ]
}
""";

    private static readonly Schema ParsedSchema = Schema.Parse(SchemaJson);

    /// <summary>
    ///     Gets the Avro schema for the variant record.
    /// </summary>
    public Schema Schema => ParsedSchema;

    /// <summary>
    ///     Gets or sets the variant identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the SKU.
    /// </summary>
    public string? Sku { get; set; }

    /// <summary>
    ///     Gets or sets the optional price.
    /// </summary>
    public AvroDecimal? Price { get; set; }

    /// <summary>
    ///     Gets or sets the optional discount.
    /// </summary>
    public DiscountInformations? Discount { get; set; }

    /// <summary>
    ///     Gets or sets the optional status.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    ///     Gets or sets the optional variant attributes.
    /// </summary>
    public IList<VariantAttribute>? Attributes { get; set; }

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
            0 => Id,
            1 => Sku,
            2 => Price,
            3 => Discount,
            4 => Status,
            5 => Attributes,
            _ => throw new AvroRuntimeException($"Bad index {fieldPos} in {nameof(Variant)}"),
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
                Id = fieldValue switch
                {
                    Guid guid => guid,
                    string value => Guid.Parse(value),
                    _ => Guid.Empty,
                };
                break;
            case 1:
                Sku = fieldValue?.ToString() ?? string.Empty;
                break;
            case 2:
                Price = fieldValue switch
                {
                    null => null,
                    AvroDecimal value => value,
                    decimal value => AvroDecimalSerializer.Serialize(value),
                    byte[] value => AvroDecimalSerializer.FromBytes(value),
                    IReadOnlyList<byte> value => AvroDecimalSerializer.FromBytes(value.ToArray()),
                    _ => Price,
                };
                break;
            case 3:
                Discount = fieldValue as DiscountInformations;
                break;
            case 4:
                Status = fieldValue?.ToString();
                break;
            case 5:
                Attributes = fieldValue switch
                {
                    null => null,
                    IList<VariantAttribute> values => values,
                    IEnumerable<VariantAttribute> values => values.ToList(),
                    _ => null,
                };
                break;
            default:
                throw new AvroRuntimeException($"Bad index {fieldPos} in {nameof(Variant)}");
        }
    }
}
