Manufacturer Domain Increment Plan
Summary
Implement only the Manufacturer domain layer in this increment, using a TDD tracer-bullet approach and stopping before Application, Infrastructure, API, or Kafka work.

Reference set for this increment:

docs/prd.md
docs/milestones/manufacturer_design.md
docs/emit_usage.md as the actual tracked Emit reference file
docs/architecture/emit_usage.md does not exist in tracked source
The goal of this increment is to leave Core with a complete, test-backed Manufacturer domain model and repository contract that later layers can build on without revisiting domain rules.

Scope For This Increment
Implement in Celeste.Inventory.Core
AuditableEntity
CreatedBy, CreatedAt
LastUpdatedBy, LastUpdatedAt
DeletedBy, DeletedAt
IsDeleted
Manufacturer
Id
Name
ContactEmail
ContactPhone
inherited audit fields
Domain behavior for:
create
update
soft delete
Deterministic normalization helper/rule for manufacturer names
trim
culture-invariant case normalization
IManufacturerRepository
only the interface, no implementation
Domain exceptions/failure types needed by later handlers
duplicate manufacturer name
manufacturer not found if the team wants this shared in Core
Do not implement yet
Common DTOs
Emit mediator requests/handlers
Mongo documents/repositories/indexes
API controllers/validators
Kafka events/producers/consumers
appsettings or installer wiring outside Core
test coverage for anything outside the domain layer
Domain Behavior To Lock In Now
Manufacturer Id is backend-generated.
Name is required after trimming.
Name must be stored trimmed.
Name comparison rules are deterministic, culture-invariant, and case-insensitive.
The domain should expose a normalized form suitable for later uniqueness/search logic.
ContactEmail and ContactPhone remain plain optional strings in the domain.
format validation belongs later in API/application validation, not in the domain model for this increment
CreatedAt, LastUpdatedAt, and DeletedAt use UTC values supplied by callers.
Audit user fields are nullable string.
Soft delete marks the entity inactive through audit fields and IsDeleted.
Re-deleting an already deleted manufacturer should fail in a way later layers can map to 404.
TDD Execution Plan
Tracer bullet 1
Add a domain test proving a manufacturer can be created with:
generated Id
trimmed Name
optional contact fields
UTC creation audit fields
Implement the minimum Manufacturer and AuditableEntity code to pass.
Tracer bullet 2
Add a test proving create rejects null/empty/whitespace-only names.
Implement the minimum name guard logic.
Tracer bullet 3
Add a test proving the domain exposes deterministic normalized name behavior.
Implement the normalization helper/rule.
Tracer bullet 4
Add a test proving update:
changes Name, ContactEmail, ContactPhone
trims and normalizes the new name
updates LastUpdatedAt and LastUpdatedBy
preserves CreatedAt and CreatedBy
Implement only the minimum update behavior.
Tracer bullet 5
Add a test proving update rejects invalid names after trimming.
Implement only the missing guard path.
Tracer bullet 6
Add a test proving soft delete:
sets IsDeleted
sets DeletedAt and DeletedBy
does not clear prior audit fields
Implement the minimum delete behavior.
Tracer bullet 7
Add a test proving deleting an already deleted manufacturer fails and does not change delete audit fields.
Implement the minimum protection for repeated delete.
Tracer bullet 8
Add tests for repository-facing domain contracts only if needed to pin behavior assumptions.
Add IManufacturerRepository and any shared domain exceptions/failure types once the entity behavior is stable.
Refactor pass
Extract shared normalization/guard helpers only after tests are green.
Keep the public surface small and avoid speculative abstractions.
Files / Artifacts Expected From This Increment
Core domain file(s) for:
AuditableEntity
Manufacturer
name normalization helper/value helper if needed
IManufacturerRepository
minimal domain exceptions/failure types
A new Core-focused test project if the repo still has no tests
domain tests only for Manufacturer behavior
Test Cases To Cover In This Increment
Create succeeds with valid data.
Create trims Name.
Create rejects null, empty, and whitespace-only Name.
Normalized name is deterministic and culture-invariant.
Update succeeds with valid data.
Update trims and normalizes Name.
Update preserves create audit fields.
Update rejects invalid Name.
Delete marks the entity as deleted.
Delete records UTC delete audit fields.
Delete on an already deleted entity fails without modifying audit fields.
Assumptions
Domain methods may accept timestamps and user values from callers rather than reaching for system time directly.
Email and phone format validation are intentionally deferred to later layers.
The domain should prepare normalized-name behavior now even though uniqueness enforcement will happen later in Application/Infrastructure.
If a shared not-found exception is not useful yet, only the duplicate/deleted-operation failure types needed by domain behavior should be introduced now.
The first test project can target domain behavior only; broader integration harnesses are deferred.
Risks And Guardrails
Do not pull validation rules like email format or phone regex into Core yet; that would blur domain and API concerns.
Do not add repository implementations, Mongo document types, or Emit wiring in this increment.
Do not introduce a generic entity base beyond the concrete AuditableEntity the milestone needs.
Do not over-model normalization as a reusable framework; a small deterministic helper is enough for now.