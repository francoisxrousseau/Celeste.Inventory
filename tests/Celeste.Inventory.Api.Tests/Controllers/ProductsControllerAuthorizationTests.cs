namespace Celeste.Inventory.Api.Tests.Controllers;

using Celeste.Inventory.Api.Authorization;
using Celeste.Inventory.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>
///     Verifies product endpoint authorization metadata.
/// </summary>
public sealed class ProductsControllerAuthorizationTests
{
    /// <summary>
    ///     Ensures each product action declares the expected authorization policy.
    /// </summary>
    [Theory]
    [InlineData(nameof(ProductsController.ListAsync), AuthorizationPolicies.ProductRead)]
    [InlineData(nameof(ProductsController.GetByIdAsync), AuthorizationPolicies.ProductRead)]
    [InlineData(nameof(ProductsController.CreateAsync), AuthorizationPolicies.ProductWrite)]
    [InlineData(nameof(ProductsController.UpdateAsync), AuthorizationPolicies.ProductWrite)]
    [InlineData(nameof(ProductsController.DeleteAsync), AuthorizationPolicies.ProductWrite)]
    public void ProductAction_UsesExpectedAuthorizePolicy(string actionName, string expectedPolicy)
    {
        var method = typeof(ProductsController).GetMethod(actionName);

        Assert.NotNull(method);

        var authorizeAttribute = method!.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .Cast<AuthorizeAttribute>()
            .SingleOrDefault();

        Assert.NotNull(authorizeAttribute);
        Assert.Equal(expectedPolicy, authorizeAttribute!.Policy);
    }

    /// <summary>
    ///     Ensures each protected product action documents unauthorized and forbidden responses.
    /// </summary>
    [Theory]
    [InlineData(nameof(ProductsController.ListAsync))]
    [InlineData(nameof(ProductsController.GetByIdAsync))]
    [InlineData(nameof(ProductsController.CreateAsync))]
    [InlineData(nameof(ProductsController.UpdateAsync))]
    [InlineData(nameof(ProductsController.DeleteAsync))]
    public void ProductAction_DocumentsUnauthorizedAndForbiddenResponses(string actionName)
    {
        var method = typeof(ProductsController).GetMethod(actionName);

        Assert.NotNull(method);

        var responseTypes = method!.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: true)
            .Cast<ProducesResponseTypeAttribute>()
            .Select(attribute => attribute.StatusCode)
            .ToArray();

        Assert.Contains(StatusCodes.Status401Unauthorized, responseTypes);
        Assert.Contains(StatusCodes.Status403Forbidden, responseTypes);
    }
}
