# Authentication & Authorization Milestone Status

## Status

Completed

## Completion Evolution

1. Authentication configuration model completed
   - Added configuration-driven authentication settings for:
     - `Authority`
     - `Audience`
     - `RequireHttpsMetadata`
     - `NameClaimType`
     - `UserIdClaimType`
   - Bound through the Options pattern in the API.

2. JWT bearer authentication completed
   - Registered JWT bearer as the only authentication mechanism.
   - Applied configuration to bearer validation and name claim mapping.
   - Enabled authentication middleware in the API pipeline.

3. Scope-based authorization completed
   - Added centralized authorization policy constants:
     - `ManufacturerRead`
     - `ManufacturerWrite`
   - Implemented centralized parsing for Keycloak-style space-separated `scope` claims.
   - Registered explicit scope policies in the API.

4. Current-user abstraction completed
   - Added a Core abstraction for current-user access.
   - Added an API HTTP-context-backed implementation that resolves:
     - stable user id from the configured user-id claim type
     - display name from the configured name claim type

5. Audit identity flow completed
   - Removed acting-user identity from manufacturer command payloads.
   - Updated create, update, and delete handlers to source audit identity from the current-user abstraction.
   - Preserved audit persistence as `string` fields.

6. Controller protection completed
   - Protected manufacturer endpoints with explicit per-action policies.
   - Applied read policy to read endpoints and write policy to write endpoints.
   - Added `401` and `403` response metadata for protected actions.

7. OpenAPI bearer security wiring completed
   - Added bearer security scheme metadata to OpenAPI.
   - Added a bearer security requirement so protected API usage is represented in generated documentation.

## Final Outcome

The Authentication & Authorization milestone is complete.

Implemented outcome:
- Keycloak-based JWT bearer authentication
- Scope-based authorization using explicit manufacturer policies
- Correct handling of space-separated `scope` claims
- Configurable claim mapping for authenticated identity
- Audit user propagation through a current-user abstraction
- Thin controllers with explicit authorization metadata
- OpenAPI bearer security documentation support
