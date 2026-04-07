namespace Celeste.Inventory.Events;

using Avro;
using Avro.Specific;

/// <summary>
///     Represents the manufacturer integration event payload.
/// </summary>
public sealed class ManufacturerEvent : ISpecificRecord
{
    private const string SchemaJson = """
{
  "type": "record",
  "name": "ManufacturerEvent",
  "namespace": "Celeste.Inventory.Events",
  "fields": [
    { "name": "Id", "type": { "type": "string", "logicalType": "uuid" } },
    { "name": "Name", "type": [ "null", "string" ], "default": null },
    { "name": "ContactEmail", "type": [ "null", "string" ], "default": null },
    { "name": "ContactPhone", "type": [ "null", "string" ], "default": null },
    { "name": "EventType", "type": "string" },
    { "name": "User", "type": "string" },
    { "name": "Date", "type": { "type": "long", "logicalType": "timestamp-millis" } }
  ]
}
""";

    private static readonly Schema ParsedSchema = Schema.Parse(SchemaJson);

    /// <summary>
    ///     Gets the Avro schema for the event.
    /// </summary>
    public Schema Schema => ParsedSchema;

    /// <summary>
    ///     Gets or sets the manufacturer identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the manufacturer name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the contact email.
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    ///     Gets or sets the contact phone.
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    ///     Gets or sets the event type.
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the user responsible for the event.
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the UTC event date.
    /// </summary>
    public DateTime Date { get; set; }

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
            1 => Name,
            2 => ContactEmail,
            3 => ContactPhone,
            4 => EventType,
            5 => User,
            6 => Date,
            _ => throw new AvroRuntimeException($"Bad index {fieldPos} in {nameof(ManufacturerEvent)}"),
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
                Name = fieldValue?.ToString();
                break;
            case 2:
                ContactEmail = fieldValue?.ToString();
                break;
            case 3:
                ContactPhone = fieldValue?.ToString();
                break;
            case 4:
                EventType = fieldValue?.ToString() ?? string.Empty;
                break;
            case 5:
                User = fieldValue?.ToString() ?? string.Empty;
                break;
            case 6:
                Date = fieldValue switch
                {
                    DateTime value => value,
                    long milliseconds => DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime,
                    _ => DateTime.MinValue,
                };
                break;
            default:
                throw new AvroRuntimeException($"Bad index {fieldPos} in {nameof(ManufacturerEvent)}");
        }
    }
}
