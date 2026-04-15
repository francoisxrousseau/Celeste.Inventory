namespace Celeste.Inventory.Api.OpenApi;

using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

/// <summary>
///     Adds OpenAPI examples for inventory controller endpoints.
/// </summary>
public sealed class InventoryOpenApiExamplesTransformer : IOpenApiDocumentTransformer
{
    /// <summary>
    ///     Adds request and response examples to inventory operations.
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
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        AddManufacturerExamples(document);
        AddProductExamples(document);
        AddVariantExamples(document);

        return Task.CompletedTask;
    }

    private static void AddManufacturerExamples(OpenApiDocument document)
    {
        SetRequestExample(document, "/manufacturers", HttpMethod.Post, ManufacturerExamples.Request);
        SetResponseExample(document, "/manufacturers", HttpMethod.Post, "201", ManufacturerExamples.Response);
        SetResponseExamples(
            document,
            "/manufacturers",
            HttpMethod.Get,
            "200",
            ("paged", "Paged manufacturers", ManufacturerExamples.PagedResponse),
            ("countOnly", "Manufacturer count", ManufacturerExamples.CountResponse));
        SetResponseExample(document, "/manufacturers/{id}", HttpMethod.Get, "200", ManufacturerExamples.Response);
        SetRequestExample(document, "/manufacturers/{id}", HttpMethod.Put, ManufacturerExamples.Request);
        SetResponseExample(document, "/manufacturers/{id}", HttpMethod.Put, "200", ManufacturerExamples.Response);
    }

    private static void AddProductExamples(OpenApiDocument document)
    {
        SetRequestExample(document, "/products", HttpMethod.Post, ProductExamples.CreateRequest);
        SetResponseExample(document, "/products", HttpMethod.Post, "201", ProductExamples.Response);
        SetResponseExamples(
            document,
            "/products",
            HttpMethod.Get,
            "200",
            ("paged", "Paged products", ProductExamples.PagedResponse),
            ("countOnly", "Product count", ProductExamples.CountResponse));
        SetResponseExample(document, "/products/{id}", HttpMethod.Get, "200", ProductExamples.Response);
        SetRequestExample(document, "/products/{id}", HttpMethod.Put, ProductExamples.UpdateRequest);
        SetResponseExample(document, "/products/{id}", HttpMethod.Put, "200", ProductExamples.Response);
    }

    private static void AddVariantExamples(OpenApiDocument document)
    {
        SetRequestExample(document, "/products/{productId}/variants", HttpMethod.Post, VariantExamples.Request);
        SetResponseExample(document, "/products/{productId}/variants", HttpMethod.Post, "201", VariantExamples.Response);
        SetResponseExample(document, "/products/{productId}/variants", HttpMethod.Get, "200", VariantExamples.ListResponse);
        SetResponseExample(document, "/products/{productId}/variants/{variantId}", HttpMethod.Get, "200", VariantExamples.Response);
        SetRequestExample(document, "/products/{productId}/variants/{variantId}", HttpMethod.Put, VariantExamples.Request);
        SetResponseExample(document, "/products/{productId}/variants/{variantId}", HttpMethod.Put, "200", VariantExamples.Response);
    }

    private static void SetRequestExample(
        OpenApiDocument document,
        string path,
        HttpMethod method,
        string json)
    {
        var operation = FindOperation(document, path, method);
        var content = operation?.RequestBody?.Content;
        if (content is null || !content.TryGetValue("application/json", out var mediaType))
            return;

        mediaType.Example = Json(json);
    }

    private static void SetResponseExample(
        OpenApiDocument document,
        string path,
        HttpMethod method,
        string statusCode,
        string json)
    {
        var operation = FindOperation(document, path, method);
        if (operation?.Responses is null || !operation.Responses.TryGetValue(statusCode, out var response))
            return;

        var content = response.Content;
        if (content is null || !content.TryGetValue("application/json", out var mediaType))
            return;

        mediaType.Example = Json(json);
    }

    private static void SetResponseExamples(
        OpenApiDocument document,
        string path,
        HttpMethod method,
        string statusCode,
        params (string Name, string Summary, string Json)[] examples)
    {
        var operation = FindOperation(document, path, method);
        if (operation?.Responses is null || !operation.Responses.TryGetValue(statusCode, out var response))
            return;

        var content = response.Content;
        if (content is null || !content.TryGetValue("application/json", out var mediaType))
            return;

        mediaType.Examples ??= new Dictionary<string, IOpenApiExample>();

        foreach (var example in examples)
        {
            mediaType.Examples[example.Name] = new OpenApiExample
            {
                Summary = example.Summary,
                Value = Json(example.Json)
            };
        }
    }

    private static OpenApiOperation? FindOperation(
        OpenApiDocument document,
        string path,
        HttpMethod method)
    {
        if (!document.Paths.TryGetValue(path, out var pathItem))
            return null;

        if (pathItem.Operations is null || !pathItem.Operations.TryGetValue(method, out var operation))
            return null;

        return operation;
    }

    private static JsonNode Json(string json)
    {
        return JsonNode.Parse(json)!;
    }

    private static class ManufacturerExamples
    {
        public const string Request = """
        {
          "name": "Celeste Apparel Co.",
          "contactEmail": "inventory@celeste.example",
          "contactPhone": "+14165550100"
        }
        """;

        public const string Response = """
        {
          "id": "22222222-2222-2222-2222-222222222222",
          "name": "Celeste Apparel Co.",
          "contactEmail": "inventory@celeste.example",
          "contactPhone": "+14165550100"
        }
        """;

        public const string PagedResponse = """
        {
          "items": [
            {
              "id": "22222222-2222-2222-2222-222222222222",
              "name": "Celeste Apparel Co.",
              "contactEmail": "inventory@celeste.example",
              "contactPhone": "+14165550100"
            }
          ],
          "totalCount": 1,
          "pageNumber": 1,
          "pageSize": 20
        }
        """;

        public const string CountResponse = """
        {
          "totalCount": 1
        }
        """;
    }

    private static class ProductExamples
    {
        public const string CreateRequest = """
        {
          "manufacturerId": "22222222-2222-2222-2222-222222222222",
          "name": "Celeste Tee",
          "description": "Soft cotton shirt",
          "status": 1,
          "category": 1,
          "tags": [
            "apparel",
            "cotton"
          ],
          "variant": {
            "sku": "TEE-RED-M",
            "price": 24.99,
            "discountInformations": {
              "discountPercentage": 15,
              "discountStartAtUtc": "2026-04-14T12:00:00Z",
              "discountEndAtUtc": "2026-04-21T12:00:00Z"
            },
            "status": 1,
            "attributes": [
              {
                "name": "Color",
                "value": "Red"
              },
              {
                "name": "Size",
                "value": "M"
              }
            ]
          }
        }
        """;

        public const string UpdateRequest = """
        {
          "manufacturerId": "22222222-2222-2222-2222-222222222222",
          "name": "Celeste Tee",
          "description": "Soft cotton shirt",
          "status": 1,
          "category": 1,
          "tags": [
            "apparel",
            "cotton",
            "featured"
          ]
        }
        """;

        public const string Response = """
        {
          "id": "33333333-3333-3333-3333-333333333333",
          "manufacturerId": "22222222-2222-2222-2222-222222222222",
          "name": "Celeste Tee",
          "description": "Soft cotton shirt",
          "status": 1,
          "category": 1,
          "tags": [
            "apparel",
            "cotton"
          ],
          "variants": [
            {
              "id": "44444444-4444-4444-4444-444444444444",
              "sku": "TEE-RED-M",
              "price": 24.99,
              "discountInformations": {
                "discountPercentage": 15,
                "discountStartAtUtc": "2026-04-14T12:00:00Z",
                "discountEndAtUtc": "2026-04-21T12:00:00Z"
              },
              "status": 1,
              "attributes": [
                {
                  "name": "Color",
                  "value": "Red"
                },
                {
                  "name": "Size",
                  "value": "M"
                }
              ]
            }
          ]
        }
        """;

        public const string PagedResponse = """
        {
          "items": [
            {
              "id": "33333333-3333-3333-3333-333333333333",
              "manufacturerId": "22222222-2222-2222-2222-222222222222",
              "name": "Celeste Tee",
              "description": "Soft cotton shirt",
              "status": 1,
              "category": 1,
              "tags": [
                "apparel",
                "cotton"
              ],
              "variants": [
                {
                  "id": "44444444-4444-4444-4444-444444444444",
                  "sku": "TEE-RED-M",
                  "price": 24.99,
                  "discountInformations": null,
                  "status": 1,
                  "attributes": [
                    {
                      "name": "Color",
                      "value": "Red"
                    }
                  ]
                }
              ]
            }
          ],
          "totalCount": 1,
          "pageNumber": 1,
          "pageSize": 20
        }
        """;

        public const string CountResponse = """
        {
          "totalCount": 1
        }
        """;
    }

    private static class VariantExamples
    {
        public const string Request = """
        {
          "sku": "TEE-RED-M",
          "price": 24.99,
          "discountInformations": {
            "discountPercentage": 15,
            "discountStartAtUtc": "2026-04-14T12:00:00Z",
            "discountEndAtUtc": "2026-04-21T12:00:00Z"
          },
          "status": 1,
          "attributes": [
            {
              "name": "Color",
              "value": "Red"
            },
            {
              "name": "Size",
              "value": "M"
            }
          ]
        }
        """;

        public const string Response = """
        {
          "id": "44444444-4444-4444-4444-444444444444",
          "sku": "TEE-RED-M",
          "price": 24.99,
          "discountInformations": {
            "discountPercentage": 15,
            "discountStartAtUtc": "2026-04-14T12:00:00Z",
            "discountEndAtUtc": "2026-04-21T12:00:00Z"
          },
          "status": 1,
          "attributes": [
            {
              "name": "Color",
              "value": "Red"
            },
            {
              "name": "Size",
              "value": "M"
            }
          ]
        }
        """;

        public const string ListResponse = """
        [
          {
            "id": "44444444-4444-4444-4444-444444444444",
            "sku": "TEE-RED-M",
            "price": 24.99,
            "discountInformations": {
              "discountPercentage": 15,
              "discountStartAtUtc": "2026-04-14T12:00:00Z",
              "discountEndAtUtc": "2026-04-21T12:00:00Z"
            },
            "status": 1,
            "attributes": [
              {
                "name": "Color",
                "value": "Red"
              }
            ]
          }
        ]
        """;
    }
}
