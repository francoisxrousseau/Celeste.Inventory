namespace Celeste.Inventory.Api.Models.Manufacturers;

/// <summary>
///     Represents a manufacturer returned by the manufacturer API.
/// </summary>
/// <param name="Id">
///     The manufacturer identifier.
/// </param>
/// <param name="Name">
///     The manufacturer display name.
/// </param>
/// <param name="ContactEmail">
///     The optional contact email.
/// </param>
/// <param name="ContactPhone">
///     The optional contact phone number.
/// </param>
public sealed record ManufacturerResponse(
    Guid Id,
    string Name,
    string? ContactEmail,
    string? ContactPhone);
