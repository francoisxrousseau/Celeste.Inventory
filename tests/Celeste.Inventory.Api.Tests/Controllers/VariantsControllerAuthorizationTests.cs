namespace Celeste.Inventory.Api.Tests.Controllers;

using Celeste.Inventory.Api.Authorization;
using Celeste.Inventory.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>
///     Verifies variant endpoint authorization metadata.
/// </summary>
public sealed class VariantsControllerAuthorizationTests
{
    /// <summary>
    ///     Ensures each variant action declares the expected authorization policy.
    /// </summary>
    [Theory]
    [InlineData(nameof(VariantsController.ListAsync), AuthorizationPolicies.ProductRead)]
    [InlineData(nameof(VariantsController.GetByIdAsync), AuthorizationPolicies.ProductRead)]
    [InlineData(nameof(VariantsController.CreateAsync), AuthorizationPolicies.ProductWrite)]
    [InlineData(nameof(VariantsController.UpdateAsync), AuthorizationPolicies.ProductWrite)]
    [InlineData(nameof(VariantsController.DeleteAsync), AuthorizationPolicies.ProductWrite)]
    public void VariantAction_UsesExpectedAuthorizePolicy(string actionName, string expectedPolicy)
    {
        var method = typeof(VariantsController).GetMethod(actionName);

        Assert.NotNull(method);

        var authorizeAttribute = method!.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .Cast<AuthorizeAttribute>()
            .SingleOrDefault();

        Assert.NotNull(authorizeAttribute);
        Assert.Equal(expectedPolicy, authorizeAttribute!.Policy);
    }

    /// <summary>
    ///     Ensures each protected variant action documents unauthorized and forbidden responses.
    /// </summary>
    [Theory]
    [InlineData(nameof(VariantsController.ListAsync))]
    [InlineData(nameof(VariantsController.GetByIdAsync))]
    [InlineData(nameof(VariantsController.CreateAsync))]
    [InlineData(nameof(VariantsController.UpdateAsync))]
    [InlineData(nameof(VariantsController.DeleteAsync))]
    public void VariantAction_DocumentsUnauthorizedAndForbiddenResponses(string actionName)
    {
        var method = typeof(VariantsController).GetMethod(actionName);

        Assert.NotNull(method);

        var responseTypes = method!.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: true)
            .Cast<ProducesResponseTypeAttribute>()
            .Select(attribute => attribute.StatusCode)
            .ToArray();

        Assert.Contains(StatusCodes.Status401Unauthorized, responseTypes);
        Assert.Contains(StatusCodes.Status403Forbidden, responseTypes);
    }
}
