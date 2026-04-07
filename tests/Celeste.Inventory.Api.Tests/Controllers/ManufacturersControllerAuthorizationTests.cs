using Celeste.Inventory.Api.Authorization;
using Celeste.Inventory.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Celeste.Inventory.Api.Tests.Controllers;

/// <summary>
///     Verifies manufacturer endpoint authorization metadata.
/// </summary>
public sealed class ManufacturersControllerAuthorizationTests
{
    /// <summary>
    ///     Ensures each manufacturer action declares the expected authorization policy.
    /// </summary>
    [Theory]
    [InlineData(nameof(ManufacturersController.ListAsync), AuthorizationPolicies.ManufacturerRead)]
    [InlineData(nameof(ManufacturersController.GetByIdAsync), AuthorizationPolicies.ManufacturerRead)]
    [InlineData(nameof(ManufacturersController.CreateAsync), AuthorizationPolicies.ManufacturerWrite)]
    [InlineData(nameof(ManufacturersController.UpdateAsync), AuthorizationPolicies.ManufacturerWrite)]
    [InlineData(nameof(ManufacturersController.DeleteAsync), AuthorizationPolicies.ManufacturerWrite)]
    public void ManufacturerAction_UsesExpectedAuthorizePolicy(string actionName, string expectedPolicy)
    {
        var method = typeof(ManufacturersController).GetMethod(actionName);

        Assert.NotNull(method);

        var authorizeAttribute = method!.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .Cast<AuthorizeAttribute>()
            .SingleOrDefault();

        Assert.NotNull(authorizeAttribute);
        Assert.Equal(expectedPolicy, authorizeAttribute!.Policy);
    }

    /// <summary>
    ///     Ensures each protected manufacturer action documents unauthorized and forbidden responses.
    /// </summary>
    [Theory]
    [InlineData(nameof(ManufacturersController.ListAsync))]
    [InlineData(nameof(ManufacturersController.GetByIdAsync))]
    [InlineData(nameof(ManufacturersController.CreateAsync))]
    [InlineData(nameof(ManufacturersController.UpdateAsync))]
    [InlineData(nameof(ManufacturersController.DeleteAsync))]
    public void ManufacturerAction_DocumentsUnauthorizedAndForbiddenResponses(string actionName)
    {
        var method = typeof(ManufacturersController).GetMethod(actionName);

        Assert.NotNull(method);

        var responseTypes = method!.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: true)
            .Cast<ProducesResponseTypeAttribute>()
            .Select(attribute => attribute.StatusCode)
            .ToArray();

        Assert.Contains(StatusCodes.Status401Unauthorized, responseTypes);
        Assert.Contains(StatusCodes.Status403Forbidden, responseTypes);
    }
}
