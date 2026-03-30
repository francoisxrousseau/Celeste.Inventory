# Celeste.Inventory.Common.md
project:
  name: Common
  description: |
    This project implements the shared/common layer. It contains shared DTOs, models,
    parameters, and response objects used across the solution. It must not reference
    any internal solution project and may be referenced by any project in the solution.
  isolation: |
    No project references to other Celeste.Inventory projects are permitted.
    This project must remain a pure dependency — it cannot create circular references.

sdk_exposure:
  description: |
    Types defined here are exposed indirectly through the Celeste.Inventory.Api.Sdk project,
    allowing NuGet consumers of the SDK to send and receive properly structured data
    without taking a direct dependency on this project.

folder_structure:
  description: |
    Each folder has a specific responsibility — place types in the correct folder.
  layout:
    Responses/:
      purpose: Output contracts returned by API endpoints and Application handlers.
      naming: FooResponse — matches the use case or resource name.
    Parameters/:
      purpose: |
        Input parameters for queries and collection endpoints, such as filters,
        sorting, and pagination. Pagination parameters must be defined here and
        reused across all paginated endpoints.
      naming: FooParameters or FooQueryParameters.
    Models/:
      purpose: Shared data transfer objects not specific to a single request or response.
      naming: FooDto or FooModel.
    Enums/:
      purpose: Shared enumerations referenced across multiple projects.
      naming: PascalCase enum names; no suffix required.

constraints:
  - No business logic, validation logic, or service implementations.
  - Types are pure data contracts — properties only, no methods with behaviour.
  - All public types and members must have XML doc comments (/// <summary>).
