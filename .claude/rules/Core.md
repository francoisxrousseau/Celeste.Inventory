# Celeste.Inventory.Core.md
project:
  name: Core
  description: |
    This project implements the core/domain layer.
    It defines domain entities, value objects, aggregates, and abstractions needed across the solution.

folder_structure:
  layout:
    Domain/:
      purpose: |
        All domain entities, value objects, and aggregates. These are the source of truth
        for business concepts and must contain no persistence or infrastructure concerns.
    Repositories/:
      purpose: |
        Repository and unit-of-work interfaces. Infrastructure implements these;
        Core must never contain any database access logic.
    Options/:
      purpose: |
        Configuration options classes required by domain services or abstractions defined in Core.
        Must follow the Options pattern — resolved via IOptions<T>, never bound directly.

domain_objects:
  description: |
    All domain entities, value objects, and aggregates must be defined in Domain/.
    These objects represent the source of truth for business logic and must not contain
    persistence or infrastructure concerns.

interfaces:
  description: |
    All repository and database interfaces must be defined in Repositories/.
    Infrastructure implements these interfaces; Core must not contain any database access logic.

exceptions:
  description: |
    Domain-level and application-level exceptions that need to be globally recognized are defined here.
    Core may also expose lightweight utility types used across the solution (e.g., Result<T>, Option<T>).

domain_services:
  description: |
    Domain services implementing business logic can be defined here.
    Lightweight system abstractions (e.g., ISystemTimeProvider) may be defined if needed across the solution.
    Core must not implement infrastructure services; it only defines abstractions.

rules:
  - Core must not reference Infrastructure, Application, Api, or Api.Sdk projects.
  - Only abstractions, domain objects, exceptions, and system-level utility services are allowed.
  - Domain objects must remain independent of persistence or infrastructure concerns.
  - All types must be reusable and safe for any other project in the solution to reference.
