# Celeste.Inventory.Application.md
project:
  name: Application
  description: |
    This project implements the application layer. It contains queries, commands, and their
    handlers, orchestrating domain logic for use cases. Handlers interact with the domain
    only through repository interfaces defined in Core. Direct database access, EF DbContext,
    or any infrastructure service is strictly forbidden in this project.

folder_structure:
  description: |
    Organisation must reflect the CQRS pattern.
  layout:
    - Features/Commands/   — command definitions
    - Features/Queries/    — query definitions
    - Features/Handlers/   — command and query handlers
    - Mapping/             — domain-to-response mapper classes and extension methods
    - Hubs/                — SignalR hub classes for real-time client push
    - Installers/          — DI registration for all Application services

cqrs:
  commands:
    description: |
      Commands represent intent to mutate state. Each command must have a corresponding
      handler in Features/Handlers/. Upon successful handling, a corresponding domain event
      must be produced (see events section below).
  queries:
    description: |
      Queries are read-only. Query handlers must never modify state, write to the database,
      or produce events.

handlers:
  description: |
    Handlers orchestrate use cases: they call repository interfaces, perform mapping,
    and return response objects. Business logic must not live in handlers — it belongs
    in the domain layer (Core).
  rules:
    - Repository interfaces from Core are the only permitted data access mechanism.
    - No EF DbContext, no infrastructure services, no direct database calls.
    - All handler methods must be async and accept a CancellationToken parameter.
    - Handlers return strongly-typed response objects (see responses section).

responses:
  description: |
    Handlers return response objects defined in Celeste.Inventory.Common/Responses/.
    Naming must be consistent — use the FooResponse convention matching the use case.
    Responses clearly represent the output contract for the API layer.
  mapping:
    description: |
      All mapping from domain entities to response objects is done manually.
      Mapping code must live in dedicated classes or static extension methods in the
      Mapping/ folder — never inline inside a handler.

hubs:
  description: |
    The Hubs/ folder contains ASP.NET Core SignalR hub classes used to push real-time
    notifications to connected clients. Hubs are triggered by command handlers after
    a use case completes successfully.
  rules:
    - Hubs must not contain business logic — they are notification surfaces only.
    - Hub method signatures and group conventions should be documented with XML doc comments.

events:
  status: pending
  description: |
    Whenever a command handler completes, a corresponding domain event must be produced
    via the mediation/messaging layer. This is not yet implemented and will be added
    manually. When generating a new command handler, always include the event production
    step as a TODO or implement it if the messaging infrastructure is in place.

installers:
  description: |
    All DI registrations for this project (handlers, hubs, mappers, etc.) must be
    declared in the Installers/ folder and called from Program.cs in the Api project.
