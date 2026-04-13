Variant Management Milestone Plan
Summary
Implement embedded product variants as part of the Product aggregate, using a TDD tracer-bullet approach and keeping the work aligned with the existing Product and Manufacturer patterns. Variants are not a top-level collection, so the repository, event model, and product read paths all evolve around the product aggregate.

Reference set for this increment:

docs/prd.md
docs/milestones/Variant_Management/design.md if added later
docs/emit_usage.md as the tracked Emit reference file

The goal of this increment is to leave the codebase with a complete, test-backed variant feature set that can be resumed cleanly in a later session without re-checking the already decided design.

Scope For This Increment
Implement in Celeste.Inventory.Core
AuditableEntity
Variant
Id
Sku
Price
DiscountInformations nullable object
Status
Attributes
VariantAttribute
Name
Value
IProductRepository additions for embedded variant operations
domain exceptions needed by later handlers

Implement in Celeste.Inventory.Common and Celeste.Inventory.Application
response models for variants and nested discount/attribute objects
product responses that include variants
mapping helpers for domain to response models
variant commands, queries, and handlers

Implement in Celeste.Inventory.Infrastructure
product document embedding for variants
Mongo mapping for variant documents
atomic repository writes for embedded variant operations

Implement in Celeste.Inventory.Api
nested variant controller under /products/{productId}/variants
request validators for variant create and update payloads
explicit product.read and product.write authorization reuse

Implement in Celeste.Inventory.Events
evolve the existing product event schema to include a nullable Variant object
variant event types in ProductEventTypes
event factory and publisher updates for variant create, update, and delete

Do not implement yet
pagination for variants
a top-level Variant repository or aggregate
currency or barcode fields
variant name
unrelated refactors outside the product/variant path

Domain Behavior To Lock In
Variant is part of the Product aggregate.
Variant inherits AuditableEntity.
Variant keeps its own status, using the existing ProductStatus enum.
Variant does not carry ProductId because it is embedded.
Variant does not carry Currency, Barcode, or Name.
DiscountInformations is optional as a whole.
Attributes are optional and consist only of Name and Value.
Deleted variants remain embedded and are filtered from default reads.
Product GET by id and product list responses include active variants.

TDD Execution Plan
Tracer bullet 1
Add a Core test proving a Product can own a Variant with audit fields, discount data, and attributes.
Implement only the minimal Core domain types needed to pass.

Tracer bullet 2
Add a repository integration test proving an embedded variant persists and round-trips through IProductRepository.
Implement the Mongo document and mapping updates needed to pass.

Tracer bullet 3
Add API controller authorization and contract tests for the nested variants controller.
Implement the thin controller and request validators needed to pass.

Tracer bullet 4
Add event/schema coverage for the variant payload embedded in the product event schema.
Implement the Event project updates and publisher mapping needed to pass.

Tracer bullet 5
Verify product read paths include variants and exclude deleted variants by default.
Adjust product mapping and response models only where needed.

Tracer bullet 6
Remove any remaining pre-load patterns from variant handlers and repository writes so create, update, and delete stay atomic.
Keep the repository as the only persistence boundary for embedded variants.

Files / Artifacts Expected From This Increment
Core domain files for Variant, DiscountInformations, and VariantAttribute
Repository interface updates and Mongo document/mapping updates
Application commands, queries, handlers, and mapping helpers
API controller, validators, and controller tests
Event schema and event factory updates

Test Cases To Cover In This Increment
Create variant succeeds with valid data.
Create variant rejects invalid SKU.
Update variant succeeds with valid data.
Update variant rejects invalid SKU.
Delete variant soft deletes the embedded item.
Get by id returns the active variant.
List returns only active variants unless deleted items are requested.
Product reads include active variants.
Repository writes do not require a separate pre-load of the product aggregate.

Assumptions
IProductRepository is the correct repository interface to extend.
Variants should return the updated product aggregate from atomic write operations when the caller needs the embedded item after the write.
Deleted variants remain embedded so they can be included when requested.
The product event schema should remain the single event contract for both product and variant changes.

Risks And Guardrails
Do not add a separate variant aggregate root.
Do not reintroduce a pre-load of the product in handlers when the repository can enforce existence with a single atomic write.
Do not broad-refactor unrelated controller or repository patterns.
Keep controllers thin and keep mapping logic in the existing mapping helpers.
