using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Celeste.Inventory.Api.OpenApi;

/// <summary>
///     Adds bearer authentication metadata to the OpenAPI document.
/// </summary>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    /// <summary>
    ///     Gets the OpenAPI security scheme name used for bearer authentication.
    /// </summary>
    public const string SchemeName = "Bearer";

    /// <summary>
    ///     Adds the bearer security scheme and requirement to the OpenAPI document.
    /// </summary>
    /// <param name="document">
    ///     The document being transformed.
    /// </param>
    /// <param name="context">
    ///     The current transformation context.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///     A completed task when the transformation finishes.
    /// </returns>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        if (document.Components.SecuritySchemes is null)
        {
            document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>();
        }

        document.Security ??= [];

        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization"
        };

        document.Security.Add(new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(SchemeName, document)
            ] = []
        });

        return Task.CompletedTask;
    }
}
