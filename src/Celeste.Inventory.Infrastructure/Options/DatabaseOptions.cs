namespace Celeste.Inventory.Infrastructure.Options;

/// <summary>
///     Represents MongoDB configuration used by the infrastructure layer.
/// </summary>
public sealed class DatabaseOptions
{
    /// <summary>
    ///     Gets the configuration section name for database options.
    /// </summary>
    public const string SectionName = "Database";

    /// <summary>
    ///     Gets or sets the MongoDB connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the MongoDB database name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
