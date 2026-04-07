namespace Celeste.Inventory.Infrastructure.Mapping;

using Celeste.Inventory.Core.Domain;
using Celeste.Inventory.Infrastructure.Documents;

/// <summary>
///     Provides mappings between manufacturer documents and domain entities.
/// </summary>
public static class ManufacturerMappingExtensions
{
    /// <summary>
    ///     Maps a manufacturer document to a domain entity.
    /// </summary>
    /// <param name="document">
    ///     The document to map.
    /// </param>
    /// <returns>
    ///     The mapped manufacturer domain entity.
    /// </returns>
    public static Manufacturer ToDomain(this ManufacturerDocument document)
    {
        return new Manufacturer
        {
            Id = document.Id,
            Name = document.Name,
            ContactEmail = document.ContactEmail,
            ContactPhone = document.ContactPhone,
            CreatedBy = document.CreatedBy,
            CreatedAt = document.CreatedAt,
            LastUpdatedBy = document.LastUpdatedBy,
            LastUpdatedAt = document.LastUpdatedAt,
            DeletedBy = document.DeletedBy,
            DeletedAt = document.DeletedAt,
        };
    }

    /// <summary>
    ///     Maps a manufacturer domain entity to a Mongo document.
    /// </summary>
    /// <param name="manufacturer">
    ///     The manufacturer to map.
    /// </param>
    /// <returns>
    ///     The mapped document.
    /// </returns>
    public static ManufacturerDocument ToDocument(this Manufacturer manufacturer)
    {
        return new ManufacturerDocument
        {
            Id = manufacturer.Id,
            Name = manufacturer.Name,
            NormalizedName = Manufacturer.NormalizeSearchText(manufacturer.Name) ?? string.Empty,
            ContactEmail = manufacturer.ContactEmail,
            ContactPhone = manufacturer.ContactPhone,
            CreatedBy = manufacturer.CreatedBy,
            CreatedAt = manufacturer.CreatedAt,
            LastUpdatedBy = manufacturer.LastUpdatedBy,
            LastUpdatedAt = manufacturer.LastUpdatedAt,
            DeletedBy = manufacturer.DeletedBy,
            DeletedAt = manufacturer.DeletedAt,
        };
    }
}
