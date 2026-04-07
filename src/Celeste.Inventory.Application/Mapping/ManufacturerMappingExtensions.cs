namespace Celeste.Inventory.Application.Mapping;

using Celeste.Inventory.Common.Responses;
using Celeste.Inventory.Core.Domain;

/// <summary>
///	Provides mapping helpers for manufacturer responses.
/// </summary>
public static class ManufacturerMappingExtensions
{
    /// <summary>
    ///	Maps a manufacturer entity to its response contract.
    /// </summary>
    /// <param name="manufacturer">
    ///	The manufacturer to map.
    /// </param>
    /// <returns>
    ///	The mapped response.
    /// </returns>
    public static ManufacturerResponse ToResponse(this Manufacturer manufacturer)
    {
        return new ManufacturerResponse(
            manufacturer.Id,
            manufacturer.Name,
            manufacturer.ContactEmail,
            manufacturer.ContactPhone);
    }
}
