namespace Celeste.Inventory.Events;

using Avro;
using Avro.Specific;

/// <summary>
///     Represents the product integration event payload.
/// </summary>
public sealed class ProductEvent : ISpecificRecord
{
    private const string SchemaJson = """
{
  "type": "record",
  "name": "ProductEvent",
  "namespace": "Celeste.Inventory.Events",
  "fields": [
    { "name": "Id", "type": { "type": "string", "logicalType": "uuid" } },
    { "name": "EventType", "type": "string" },
    { "name": "ManufacturerId", "type": [ "null", { "type": "string", "logicalType": "uuid" } ], "default": null },
    { "name": "ProductDetails", "type": [ "null", {
      "type": "record",
      "name": "ProductDetails",
      "fields": [
        { "name": "Name", "type": [ "null", "string" ], "default": null },
        { "name": "Description", "type": [ "null", "string" ], "default": null }
      ]
    }], "default": null },
    { "name": "Status", "type": [ "null", "string" ], "default": null },
    { "name": "Tags", "type": [ "null", { "type": "array", "items": "string" } ], "default": null },
    { "name": "Date", "type": { "type": "long", "logicalType": "timestamp-millis" } },
    { "name": "User", "type": "string" }
  ]
}
""";

    private static readonly Schema ParsedSchema = Schema.Parse(SchemaJson);

    /// <summary>
    ///     Gets the Avro schema for the event.
    /// </summary>
    public Schema Schema => ParsedSchema;

    /// <summary>
    ///     Gets or sets the product identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the event type.
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the optional manufacturer identifier.
    /// </summary>
    public Guid? ManufacturerId { get; set; }

    /// <summary>
    ///     Gets or sets the optional product details.
    /// </summary>
    public ProductDetails? ProductDetails { get; set; }

    /// <summary>
    ///     Gets or sets the optional status.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    ///     Gets or sets the optional tags.
    /// </summary>
    public IList<string>? Tags { get; set; }

    /// <summary>
    ///     Gets or sets the UTC event date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    ///     Gets or sets the user responsible for the event.
    /// </summary>
    public string User { get; set; } = string.Empty;

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
            1 => EventType,
            2 => ManufacturerId,
            3 => ProductDetails,
            4 => Status,
            5 => Tags,
            6 => Date,
            7 => User,
            _ => throw new AvroRuntimeException($"Bad index {fieldPos} in {nameof(ProductEvent)}"),
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
                EventType = fieldValue?.ToString() ?? string.Empty;
                break;
            case 2:
                ManufacturerId = fieldValue switch
                {
                    null => null,
                    Guid guid => guid,
                    string value when !string.IsNullOrWhiteSpace(value) => Guid.Parse(value),
                    _ => null,
                };
                break;
            case 3:
                ProductDetails = fieldValue as ProductDetails;
                break;
            case 4:
                Status = fieldValue?.ToString();
                break;
            case 5:
                Tags = fieldValue switch
                {
                    null => null,
                    IList<string> values => values,
                    IEnumerable<string> values => values.ToList(),
                    _ => null,
                };
                break;
            case 6:
                Date = fieldValue switch
                {
                    DateTime value => value,
                    long milliseconds => DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime,
                    _ => DateTime.MinValue,
                };
                break;
            case 7:
                User = fieldValue?.ToString() ?? string.Empty;
                break;
            default:
                throw new AvroRuntimeException($"Bad index {fieldPos} in {nameof(ProductEvent)}");
        }
    }
}
