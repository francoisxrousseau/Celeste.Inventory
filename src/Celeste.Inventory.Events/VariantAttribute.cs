namespace Celeste.Inventory.Events;

using Avro;
using Avro.Specific;

/// <summary>
///     Represents a variant attribute embedded in product integration events.
/// </summary>
public sealed class VariantAttribute : ISpecificRecord
{
    private const string SchemaJson = """
{
  "type": "record",
  "name": "VariantAttribute",
  "namespace": "Celeste.Inventory.Events",
  "fields": [
    { "name": "Name", "type": "string" },
    { "name": "Value", "type": "string" }
  ]
}
""";

    private static readonly Schema ParsedSchema = Schema.Parse(SchemaJson);

    /// <summary>
    ///     Gets the Avro schema for the attribute record.
    /// </summary>
    public Schema Schema => ParsedSchema;

    /// <summary>
    ///     Gets or sets the attribute name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the attribute value.
    /// </summary>
    public string Value { get; set; } = string.Empty;

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
            1 => Value,
            _ => throw new AvroRuntimeException($"Bad index {fieldPos} in {nameof(VariantAttribute)}"),
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
                Name = fieldValue?.ToString() ?? string.Empty;
                break;
            case 1:
                Value = fieldValue?.ToString() ?? string.Empty;
                break;
            default:
                throw new AvroRuntimeException($"Bad index {fieldPos} in {nameof(VariantAttribute)}");
        }
    }
}
