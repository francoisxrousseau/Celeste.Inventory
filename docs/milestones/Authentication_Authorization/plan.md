Authentication & Authorization Milestone Plan
Summary
Implement Keycloak-based JWT bearer authentication and scope-based authorization in the API layer, while exposing the authenticated user id to the application layer for audit fields. This plan excludes test work by request and focuses only on the production changes needed for the milestone.

Implementation Steps
Add the authentication configuration model

Project/folder: src/Celeste.Inventory.Api/Options
Create a dedicated options class for authentication and identity settings with:
Authority
Audience
RequireHttpsMetadata
NameClaimType with default preferred_username
UserIdClaimType with default sub
Add a section name constant so binding is centralized.
Add the new configuration section to:
src/Celeste.Inventory.Api/appsettings.json
src/Celeste.Inventory.Api/appsettings.Development.json
Keep all values configuration-driven and environment-overridable.
Register and validate auth options

Project/folder: src/Celeste.Inventory.Api/Installers
Add a dedicated installer for authentication and authorization setup.
Bind the new options section with the Options pattern.
Validate required values on startup:
Authority
Audience
UserIdClaimType
NameClaimType
Update src/Celeste.Inventory.Api/Program.cs only to call this installer, keeping startup composition consistent with the existing installer pattern.
Configure JWT bearer authentication

Project/folder: src/Celeste.Inventory.Api/Installers
Register JWT bearer as the only authentication mechanism.
Apply the bound options to bearer setup:
Authority
Audience
RequireHttpsMetadata
Configure identity/name mapping so ClaimsIdentity.Name uses the configured NameClaimType.
Set bearer as the default authenticate and challenge scheme so missing or invalid tokens produce standard 401 behavior.
Update middleware order in src/Celeste.Inventory.Api/Program.cs to call UseAuthentication() before UseAuthorization().
Add scope-based authorization

Project/folder: src/Celeste.Inventory.Api/Authorization and src/Celeste.Inventory.Api/Installers
Introduce named policy constants:
ManufacturerRead
ManufacturerWrite
Implement centralized scope parsing for the scope claim.
Ensure the parser supports Keycloak’s space-separated scope format within a single claim and does not assume one claim per scope.
Register policies that require:
manufacturer.read for ManufacturerRead
manufacturer.write for ManufacturerWrite
Keep parsing and evaluation in one reusable place so all scope policies behave consistently.
Expose the authenticated user to the application layer

Project/folder: src/Celeste.Inventory.Core and src/Celeste.Inventory.Api
Add a Core abstraction for current user access so Application can depend on it without referencing HTTP or claims APIs.
Implement the HTTP-backed version in API using IHttpContextAccessor.
Resolve:
user id from the configured UserIdClaimType
display/name value from the configured NameClaimType
Keep audit identity tied only to the configured user-id claim.
Register the implementation in API DI.
Refactor audit identity flow

Project/folder: src/Celeste.Inventory.Application/Features/Commands and src/Celeste.Inventory.Application/Features/Handlers
Remove controller responsibility for passing the acting user into commands.
Update create, update, and delete handlers to use the current-user abstraction when populating:
CreatedBy
LastUpdatedBy
DeletedBy
Simplify command contracts so they no longer carry user identity fields.
Leave audit fields as string, matching the design and current domain model.
Protect manufacturer endpoints explicitly

Project/folder: src/Celeste.Inventory.Api/Controllers
Add explicit [Authorize(Policy = ...)] attributes on each action instead of using a single controller-level policy.
Apply policies exactly as specified:
GET /manufacturers → ManufacturerRead
GET /manufacturers/{id} → ManufacturerRead
POST /manufacturers → ManufacturerWrite
PUT /manufacturers/{id} → ManufacturerWrite
DELETE /manufacturers/{id} → ManufacturerWrite
Remove use of User.Identity?.Name from controller command construction.
Add ProducesResponseType declarations for 401 and 403 so the HTTP contract reflects authentication vs authorization failures correctly.
Update OpenAPI for bearer auth

Project/folder: src/Celeste.Inventory.Api/Installers or the API OpenAPI registration point
Add bearer security definition and security requirement to the OpenAPI setup.
Ensure Swagger/OpenAPI shows that the protected endpoints require bearer authentication.
Ensure the documented responses for protected endpoints include 401 and 403.
Public Interfaces and Contract Changes
New API configuration section for authentication and claim mapping.
New authorization policy names:
ManufacturerRead
ManufacturerWrite
Manufacturer endpoints become explicitly protected and document 401 and 403.
Application command flow changes so audit user identity comes from a current-user abstraction rather than controller-supplied values.
Assumptions and Defaults
This milestone protects only manufacturer endpoints because the design document only defines manufacturer scopes and manufacturer endpoint rules.
UserIdClaimType defaults to sub and is the only claim used for persisted audit fields.
NameClaimType defaults to preferred_username and is available for identity naming, but not for audit persistence.
Scope evaluation is based on the scope claim exactly as described in the design document; no role-based authorization is added.
Test design and implementation are intentionally excluded from this plan and will be handled separately with the TDD workflow.