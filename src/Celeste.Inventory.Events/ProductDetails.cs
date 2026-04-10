namespace Celeste.Inventory.Events;

using Avro;
using Avro.Specific;

/// <summary>
///     Represents the product details section within product integration events.
/// </summary>
public sealed class ProductDetails : ISpecificRecord
{
    private const string SchemaJson = """
{
  "type": "record",
  "name": "ProductDetails",
  "namespace": "Celeste.Inventory.Events",
  "fields": [
    { "name": "Name", "type": [ "null", "string" ], "default": null },
    { "name": "Description", "type": [ "null", "string" ], "default": null }
  ]
}
""";

    private static readonly Schema ParsedSchema = Schema.Parse(SchemaJson);

    /// <summary>
    ///     Gets the Avro schema for the details record.
    /// </summary>
    public Schema Schema => ParsedSchema;

    /// <summary>
    ///     Gets or sets the product name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the optional product description.
    /// </summary>
    public string? Description { get; set; }

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
            0 => Name,
            1 => Description,
            _ => throw new AvroRuntimeException($"Bad index {fieldPos} in {nameof(ProductDetails)}"),
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
                Name = fieldValue?.ToString();
                break;
            case 1:
                Description = fieldValue?.ToString();
                break;
            default:
                throw new AvroRuntimeException($"Bad index {fieldPos} in {nameof(ProductDetails)}");
        }
    }
}
