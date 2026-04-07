namespace Celeste.Inventory.Api.Models.Manufacturers;

/// <summary>
///	Represents the payload used to update a manufacturer.
/// </summary>
public sealed class UpdateManufacturerRequest
{
    /// <summary>
    ///	Gets or sets the manufacturer name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///	Gets or sets the optional contact email.
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    ///	Gets or sets the optional contact phone number.
    /// </summary>
    public string? ContactPhone { get; set; }
}
