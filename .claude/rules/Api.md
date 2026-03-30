# Celeste.Inventory.Api.md
project:
  name: API
  description: |
    This project contains the root Program.cs of the solution and is responsible
    for configuring all solution-level services, third-party libraries, and internal installers.
    It can reference all solution projects except those explicitly meant to remain isolated.

authentication:
  type: jwt
  description: |
    Authentication must support an external identity provider with JWT.
    Configuration should allow setting Authority and Audience values.

authorization:
  description: |
    Controllers must apply [Authorize] where access control is required.
    Authorization policies must be registered in DI via Installers/ and named using constants.
  swagger:
    description: |
      Swagger must be configured with AddSecurityDefinition and AddSecurityRequirement
      to support JWT bearer token entry for interactive testing.

mediation:
  description: |
    All business logic, commands, and events must be processed through a mediation layer
    that supports event publishing and an outbox pattern.
    Controllers must never handle events, messaging libraries, or database operations directly.
  notes: |
    Implementation is hand-rolled; no specific NuGet library is enforced by this rule.

messaging:
  description: |
    The Messaging/ folder contains Kafka consumer registrations — wiring consumers
    to the event pipeline at the application bootstrap level.
  notes: |
    Consumers must contain no business logic; they delegate entirely to the mediator.
    Business logic triggered by consumed messages belongs in Application handlers.

controllers:
  design:
    principles:
      - Lightweight: delegate all business logic and event handling to services or commands/queries.
      - RESTful endpoints: follow REST conventions for HTTP verbs and resource naming.
      - Validation: all parameters and models must be validated using FluentValidation.
      - Validators organization: validators must reside in the Validators/ folder and be registered in DI.
      - Pagination: GET endpoints returning collections should support pagination when applicable.
    documentation:
      swagger:
        description: |
          Controller methods must declare expected HTTP responses using ProducesResponseType.
          Inputs and outputs should be clearly represented in Swagger/OpenAPI.
        guidance: |
          Pressing 'play' should display the Swagger page for interactive API testing.

error_handling:
  description: |
    All error responses must follow the ProblemDetails format (RFC 7807).
    Register via AddProblemDetails() and UseExceptionHandler() — no custom error wrappers.
  validation:
    description: |
      FluentValidation errors must also be mapped to the ProblemDetails shape,
      ensuring a consistent error contract across all failure types.

health_checks:
  description: |
    Health check endpoints are mandatory — the service will run in Kubernetes
    and must expose liveness and readiness probes.
  implementation:
    - Register checks via AddHealthChecks() in DI.
    - Map endpoints via MapHealthChecks() (e.g. /health/live, /health/ready).

observability:
  description: |
    The API must expose a /metrics endpoint for Prometheus scraping in the Kubernetes cluster.
    Use OpenTelemetry for distributed tracing and metrics collection.
  notes: |
    OpenTelemetry registration belongs in Installers/.
    Structured logging is expected; specific logging library is not enforced by this rule.

general_guidelines:
  - Program.cs should centralize configuration for solution-level services.
  - Controllers should never perform direct event handling or database access.
  - API design must prioritize maintainability, testability, and clear separation of concerns.
