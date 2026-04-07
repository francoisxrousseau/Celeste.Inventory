namespace Celeste.Inventory.Api.Models.Manufacturers;

/// <summary>
///     Provides mapping helpers from application manufacturer responses to API response models.
/// </summary>
public static class ManufacturerResponseMappingExtensions
{
    /// <summary>
    ///     Maps an application manufacturer response to the API response model.
    /// </summary>
    /// <param name="response">
    ///     The application response to map.
    /// </param>
    /// <returns>
    ///     The API response model.
    /// </returns>
    public static ManufacturerResponse ToApiModel(this Common.Responses.ManufacturerResponse response)
    {
        return new ManufacturerResponse(
            response.Id,
            response.Name,
            response.ContactEmail,
            response.ContactPhone);
    }

    /// <summary>
    ///     Maps an application manufacturer count response to the API response model.
    /// </summary>
    /// <param name="response">
    ///     The application response to map.
    /// </param>
    /// <returns>
    ///     The API response model.
    /// </returns>
    public static ManufacturerCountResponse ToApiModel(this Common.Responses.ManufacturerCountResponse response)
    {
        return new ManufacturerCountResponse(response.TotalCount);
    }

    /// <summary>
    ///     Maps an application paged manufacturer response to the API response model.
    /// </summary>
    /// <param name="response">
    ///     The application response to map.
    /// </param>
    /// <returns>
    ///     The API response model.
    /// </returns>
    public static PagedManufacturersResponse ToApiModel(this Common.Responses.PagedManufacturersResponse response)
    {
        return new PagedManufacturersResponse(
            response.Items.Select(item => item.ToApiModel()).ToArray(),
            response.TotalCount,
            response.PageNumber,
            response.PageSize);
    }
}
