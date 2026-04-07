namespace Celeste.Inventory.Api.Tests.OpenApi;

using Celeste.Inventory.Api.OpenApi;
using Microsoft.OpenApi;

/// <summary>
///     Covers bearer security OpenAPI document transformation behavior.
/// </summary>
public sealed class BearerSecuritySchemeTransformerTests
{
    /// <summary>
    ///     Ensures the OpenAPI document exposes the bearer scheme and security requirement.
    /// </summary>
    [Fact]
    public async Task TransformAsync_AddsBearerSecuritySchemeAndRequirement()
    {
        var transformer = new BearerSecuritySchemeTransformer();
        var document = new OpenApiDocument
        {
            Components = new OpenApiComponents()
        };

        await transformer.TransformAsync(document, null!, CancellationToken.None);

        Assert.NotNull(document.Components);
        Assert.NotNull(document.Components.SecuritySchemes);
        Assert.True(document.Components.SecuritySchemes.ContainsKey(BearerSecuritySchemeTransformer.SchemeName));

        var securityScheme = document.Components.SecuritySchemes[BearerSecuritySchemeTransformer.SchemeName];
        Assert.Equal(SecuritySchemeType.Http, securityScheme.Type);
        Assert.Equal("bearer", securityScheme.Scheme);
        Assert.Equal("JWT", securityScheme.BearerFormat);

        Assert.NotNull(document.Security);
        var requirement = Assert.Single(document.Security);
        Assert.Single(requirement.Keys);
        Assert.IsType<OpenApiSecuritySchemeReference>(requirement.Keys.Single());
    }
}
