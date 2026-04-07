using Celeste.Inventory.Api.Controllers;
using Celeste.Inventory.Api.Models.Manufacturers;
using Microsoft.AspNetCore.Mvc;

namespace Celeste.Inventory.Api.Tests.Controllers;

/// <summary>
///     Verifies the documented manufacturer API contract types.
/// </summary>
public sealed class ManufacturersControllerOpenApiContractTests
{
    /// <summary>
    ///     Ensures single-manufacturer endpoints expose API response models.
    /// </summary>
    [Theory]
    [InlineData(nameof(ManufacturersController.CreateAsync), typeof(ManufacturerResponse))]
    [InlineData(nameof(ManufacturersController.GetByIdAsync), typeof(ManufacturerResponse))]
    [InlineData(nameof(ManufacturersController.UpdateAsync), typeof(ManufacturerResponse))]
    public void ManufacturerAction_UsesApiResponseModelInProducesResponseType(string actionName, Type expectedType)
    {
        var method = typeof(ManufacturersController).GetMethod(actionName);

        Assert.NotNull(method);

        var producesAttribute = method!.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: true)
            .Cast<ProducesResponseTypeAttribute>()
            .Single(attribute => attribute.Type == expectedType);

        Assert.Equal(expectedType, producesAttribute.Type);
    }

    /// <summary>
    ///     Ensures list and count endpoints expose API response models.
    /// </summary>
    [Fact]
    public void ListAsync_UsesApiResponseModelsInProducesResponseType()
    {
        var method = typeof(ManufacturersController).GetMethod(nameof(ManufacturersController.ListAsync));

        Assert.NotNull(method);

        var producesTypes = method!.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: true)
            .Cast<ProducesResponseTypeAttribute>()
            .Select(attribute => attribute.Type)
            .ToArray();

        Assert.Contains(typeof(PagedManufacturersResponse), producesTypes);
        Assert.Contains(typeof(ManufacturerCountResponse), producesTypes);
    }
}
