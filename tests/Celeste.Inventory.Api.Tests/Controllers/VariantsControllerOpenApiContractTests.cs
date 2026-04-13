namespace Celeste.Inventory.Api.Tests.Controllers;

using Celeste.Inventory.Api.Controllers;
using Celeste.Inventory.Api.Models.Products;
using Microsoft.AspNetCore.Mvc;

/// <summary>
///     Verifies the documented variant API contract types.
/// </summary>
public sealed class VariantsControllerOpenApiContractTests
{
    /// <summary>
    ///     Ensures single-variant endpoints expose API response models.
    /// </summary>
    [Theory]
    [InlineData(nameof(VariantsController.CreateAsync), typeof(VariantResponse))]
    [InlineData(nameof(VariantsController.GetByIdAsync), typeof(VariantResponse))]
    [InlineData(nameof(VariantsController.UpdateAsync), typeof(VariantResponse))]
    public void VariantAction_UsesApiResponseModelInProducesResponseType(string actionName, Type expectedType)
    {
        var method = typeof(VariantsController).GetMethod(actionName);

        Assert.NotNull(method);

        var producesAttribute = method!.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: true)
            .Cast<ProducesResponseTypeAttribute>()
            .Single(attribute => attribute.Type == expectedType);

        Assert.Equal(expectedType, producesAttribute.Type);
    }

    /// <summary>
    ///     Ensures the list endpoint exposes the API response collection type.
    /// </summary>
    [Fact]
    public void ListAsync_UsesApiResponseModelInProducesResponseType()
    {
        var method = typeof(VariantsController).GetMethod(nameof(VariantsController.ListAsync));

        Assert.NotNull(method);

        var producesAttribute = method!.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: true)
            .Cast<ProducesResponseTypeAttribute>()
            .Single(attribute => attribute.Type == typeof(IReadOnlyList<VariantResponse>));

        Assert.Equal(typeof(IReadOnlyList<VariantResponse>), producesAttribute.Type);
    }

    /// <summary>
    ///     Ensures the create endpoint targets a named route for the get-by-id location.
    /// </summary>
    [Fact]
    public void GetByIdAsync_DeclaresNamedRoute()
    {
        var method = typeof(VariantsController).GetMethod(nameof(VariantsController.GetByIdAsync));

        Assert.NotNull(method);

        var routeAttribute = method!.GetCustomAttributes(typeof(HttpGetAttribute), inherit: true)
            .Cast<HttpGetAttribute>()
            .Single();

        Assert.Equal("GetVariantById", routeAttribute.Name);
    }

    /// <summary>
    ///     Ensures the delete endpoint documents no-content responses.
    /// </summary>
    [Fact]
    public void DeleteAsync_UsesNoContentProducesResponseType()
    {
        var method = typeof(VariantsController).GetMethod(nameof(VariantsController.DeleteAsync));

        Assert.NotNull(method);

        var statusCodes = method!.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: true)
            .Cast<ProducesResponseTypeAttribute>()
            .Select(attribute => attribute.StatusCode)
            .ToArray();

        Assert.Contains(204, statusCodes);
    }
}
