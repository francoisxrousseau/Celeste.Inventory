namespace Celeste.Inventory.Api.Tests.OpenApi;

using System.Text.Json.Nodes;
using Celeste.Inventory.Api.OpenApi;
using Microsoft.OpenApi;

/// <summary>
///     Covers inventory OpenAPI example transformation behavior.
/// </summary>
public sealed class InventoryOpenApiExamplesTransformerTests
{
    /// <summary>
    ///     Ensures inventory endpoints receive request and response examples.
    /// </summary>
    [Fact]
    public async Task TransformAsync_AddsInventoryEndpointExamples()
    {
        var transformer = new InventoryOpenApiExamplesTransformer();
        var document = new OpenApiDocument
        {
            Paths = new OpenApiPaths
            {
                ["/manufacturers"] = CreatePathItem(HttpMethod.Post, HttpMethod.Get),
                ["/manufacturers/{id}"] = CreatePathItem(HttpMethod.Get, HttpMethod.Put),
                ["/products"] = CreatePathItem(HttpMethod.Post, HttpMethod.Get),
                ["/products/{id}"] = CreatePathItem(HttpMethod.Get, HttpMethod.Put),
                ["/products/{productId}/variants"] = CreatePathItem(HttpMethod.Post, HttpMethod.Get),
                ["/products/{productId}/variants/{variantId}"] = CreatePathItem(HttpMethod.Get, HttpMethod.Put)
            }
        };

        await transformer.TransformAsync(document, null!, CancellationToken.None);

        var createProductExample = RequestExample(document, "/products", HttpMethod.Post);
        var createProductVariant = Assert.IsType<JsonObject>(createProductExample["variant"]);
        var createManufacturerExample = RequestExample(document, "/manufacturers", HttpMethod.Post);
        var updateVariantExample = RequestExample(document, "/products/{productId}/variants/{variantId}", HttpMethod.Put);
        var listProductsExamples = ResponseExamples(document, "/products", HttpMethod.Get, "200");

        Assert.Equal("TEE-RED-M", createProductVariant["sku"]?.GetValue<string>());
        Assert.Equal("Celeste Apparel Co.", createManufacturerExample["name"]?.GetValue<string>());
        Assert.Equal(24.99, updateVariantExample["price"]?.GetValue<double>());
        Assert.True(listProductsExamples.ContainsKey("paged"));
        Assert.True(listProductsExamples.ContainsKey("countOnly"));
    }

    private static OpenApiPathItem CreatePathItem(params HttpMethod[] methods)
    {
        var operations = methods.ToDictionary(method => method, CreateOperation);

        return new OpenApiPathItem
        {
            Operations = operations
        };
    }

    private static OpenApiOperation CreateOperation(HttpMethod method)
    {
        return new OpenApiOperation
        {
            RequestBody = method == HttpMethod.Post || method == HttpMethod.Put
                ? new OpenApiRequestBody
                {
                    Content = JsonContent()
                }
                : null,
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse { Content = JsonContent() },
                ["201"] = new OpenApiResponse { Content = JsonContent() }
            }
        };
    }

    private static Dictionary<string, OpenApiMediaType> JsonContent()
    {
        return new Dictionary<string, OpenApiMediaType>
        {
            ["application/json"] = new OpenApiMediaType()
        };
    }

    private static JsonObject RequestExample(
        OpenApiDocument document,
        string path,
        HttpMethod method)
    {
        Assert.True(document.Paths.ContainsKey(path));
        var pathItem = document.Paths[path];
        Assert.NotNull(pathItem.Operations);
        var operations = pathItem.Operations;
        Assert.True(operations.ContainsKey(method));
        var operation = operations[method];
        Assert.NotNull(operation.RequestBody);
        var requestBody = operation.RequestBody;
        Assert.NotNull(requestBody.Content);
        var content = requestBody.Content;
        Assert.True(content.ContainsKey("application/json"));
        var mediaType = content["application/json"];

        return Assert.IsType<JsonObject>(mediaType.Example);
    }

    private static IDictionary<string, IOpenApiExample> ResponseExamples(
        OpenApiDocument document,
        string path,
        HttpMethod method,
        string statusCode)
    {
        Assert.True(document.Paths.ContainsKey(path));
        var pathItem = document.Paths[path];
        Assert.NotNull(pathItem.Operations);
        var operations = pathItem.Operations;
        Assert.True(operations.ContainsKey(method));
        var operation = operations[method];
        Assert.NotNull(operation.Responses);
        var responses = operation.Responses;
        Assert.True(responses.ContainsKey(statusCode));
        var response = responses[statusCode];
        Assert.NotNull(response.Content);
        var content = response.Content;
        Assert.True(content.ContainsKey("application/json"));
        var mediaType = content["application/json"];

        Assert.NotNull(mediaType.Examples);
        return mediaType.Examples;
    }
}
