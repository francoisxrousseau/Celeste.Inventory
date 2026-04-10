namespace Celeste.Inventory.Api.Tests.Controllers;

using Celeste.Inventory.Api.Controllers;
using Celeste.Inventory.Api.Models.Products;
using Microsoft.AspNetCore.Mvc;

/// <summary>
///     Verifies the documented product API contract types.
/// </summary>
public sealed class ProductsControllerOpenApiContractTests
{
    /// <summary>
    ///     Ensures single-product endpoints expose API response models.
    /// </summary>
    [Theory]
    [InlineData(nameof(ProductsController.CreateAsync), typeof(ProductResponse))]
    [InlineData(nameof(ProductsController.GetByIdAsync), typeof(ProductResponse))]
    [InlineData(nameof(ProductsController.UpdateAsync), typeof(ProductResponse))]
    public void ProductAction_UsesApiResponseModelInProducesResponseType(string actionName, Type expectedType)
    {
        var method = typeof(ProductsController).GetMethod(actionName);

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
        var method = typeof(ProductsController).GetMethod(nameof(ProductsController.ListAsync));

        Assert.NotNull(method);

        var producesTypes = method!.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: true)
            .Cast<ProducesResponseTypeAttribute>()
            .Select(attribute => attribute.Type)
            .ToArray();

        Assert.Contains(typeof(PagedProductsResponse), producesTypes);
        Assert.Contains(typeof(ProductCountResponse), producesTypes);
    }

    /// <summary>
    ///     Ensures the create endpoint targets a named route for the get-by-id location.
    /// </summary>
    [Fact]
    public void GetByIdAsync_DeclaresNamedRoute()
    {
        var method = typeof(ProductsController).GetMethod(nameof(ProductsController.GetByIdAsync));

        Assert.NotNull(method);

        var routeAttribute = method!.GetCustomAttributes(typeof(HttpGetAttribute), inherit: true)
            .Cast<HttpGetAttribute>()
            .Single();

        Assert.Equal("GetProductById", routeAttribute.Name);
    }

    /// <summary>
    ///     Ensures the delete endpoint documents no-content responses.
    /// </summary>
    [Fact]
    public void DeleteAsync_UsesNoContentProducesResponseType()
    {
        var method = typeof(ProductsController).GetMethod(nameof(ProductsController.DeleteAsync));

        Assert.NotNull(method);

        var statusCodes = method!.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: true)
            .Cast<ProducesResponseTypeAttribute>()
            .Select(attribute => attribute.StatusCode)
            .ToArray();

        Assert.Contains(204, statusCodes);
    }
}
