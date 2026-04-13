# Variant Management Status

## Status

In progress

## Completed

- Core domain support for embedded variants is in place.
- Product responses now include variants.
- Application commands, queries, and handlers for variant create, update, delete, get-by-id, and list are in place.
- Infrastructure persistence for embedded variants is implemented in the product repository and mapping layer.
- API controller surface for `/products/{productId}/variants` exists with explicit `product.read` and `product.write` reuse.
- Variant request validation is present in the API layer.
- Product event schema and event publishing were updated to carry a nullable embedded variant payload.
- Focused Core, Infrastructure, API, and product-controller tests were added for the variant path.

## Current Integration Point

The feature is wired end-to-end and the latest pass removed the extra product pre-load from the variant handlers and repository writes so create, update, and delete use atomic repository operations.

Current repository behavior:
- variant create uses a single product update call
- variant update uses a single product update call
- variant delete uses a single product update call
- the repository returns the updated product aggregate from write operations so the handler can publish the event without a second read

## Verification

Last known green checks:
- `dotnet build src/Celeste.Inventory.Api/Celeste.Inventory.Api.csproj --disable-build-servers -v minimal`
- `dotnet test tests/Celeste.Inventory.Infrastructure.Tests/Celeste.Inventory.Infrastructure.Tests.csproj --no-build --disable-build-servers -v minimal --filter FullyQualifiedName~AddVariantAsync_ThenGetVariantByIdAsync_PersistsAndReturnsVariant`
- `dotnet test tests/Celeste.Inventory.Api.Tests/Celeste.Inventory.Api.Tests.csproj --no-build --disable-build-servers -v minimal --filter FullyQualifiedName~VariantsController`

## Resume Next Session

1. Rebuild the API project first to confirm the integrated path is still green.
2. Re-run the targeted infrastructure variant persistence test.
3. If any behavior drift appears, start with the repository contract and handler return shapes before touching the controller.

## Notes

- Keep using targeted build-first verification before any test run.
- Do not switch back to broad test retries if the worker hangs.
- The milestone should stay aligned with the existing Product and Manufacturer patterns and avoid unrelated refactors.
