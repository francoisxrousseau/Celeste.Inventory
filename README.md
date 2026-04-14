# Celeste Inventory

Celeste Inventory is the catalog and inventory microservice for the Celeste Labs platform. It manages product definitions for a single-store inventory model and provides the foundation for tracking stock through immutable inventory movements.

The service follows a layered clean architecture: HTTP requests enter through the API, commands and queries are handled in the application layer, domain rules live in the core project, persistence is implemented in infrastructure, and integration events are declared in the events project.

## Product Scope

This service is responsible for:

- Managing manufacturers, products, and product variants.
- Tracking inventory at the variant level.
- Recording inventory changes as immutable movements.
- Grouping stock intake through deliveries.
- Integrating with external systems, such as an order service, through events.

Inventory is calculated from movement history:

```text
Current inventory = sum of all movements for a variant
```

Corrections are represented as adjustment movements. Existing movements are not modified or deleted.

## Capabilities

- Manufacturer creation, update, deletion, lookup, listing, and counting.
- Product creation, update, deletion, lookup, listing, and counting.
- Variant creation, update, deletion, lookup, and listing.
- Inventory movement recording and history.
- Delivery recording for multi-item stock intake.
- Inventory adjustments for manual corrections.
- External sales and returns ingestion.
- Current inventory lookup per variant.
- Data seeding for development and test environments.
- JWT bearer authentication with Keycloak-compatible configuration.
- OpenAPI and Swagger UI in development.
- Liveness and readiness health checks.
- OpenTelemetry-based observability.
- Kafka event publishing and consumption for inventory integrations.
- MongoDB-backed repository implementations for catalog and inventory data.

## Architecture

```text
Celeste.Inventory.Api
  -> Celeste.Inventory.Application
    -> Celeste.Inventory.Core
      -> Celeste.Inventory.Infrastructure
    -> Celeste.Inventory.Events
```

Request flow:

```text
Controller
  -> Emit mediator command/query
    -> Application handler
      -> Core repository contract
        -> Infrastructure repository
      -> Event publisher
        -> Kafka outbox/event pipeline
```

## Projects

| Project | Purpose |
| --- | --- |
| `Celeste.Inventory.Api` | ASP.NET Core API, controllers, request validation, authentication, OpenAPI, health checks, and composition root. |
| `Celeste.Inventory.Application` | CQRS commands, queries, handlers, mapping, and application-level orchestration. |
| `Celeste.Inventory.Core` | Domain entities, repository interfaces, messaging interfaces, identity abstractions, and domain exceptions. |
| `Celeste.Inventory.Infrastructure` | MongoDB document models, mappings, repository implementations, database options, and event publisher implementations. |
| `Celeste.Inventory.Common` | Shared DTOs, request parameters, response models, and common enums. |
| `Celeste.Inventory.Events` | Kafka event contracts, Avro schemas, event factories, topics, and serialization helpers. |
| `Celeste.Inventory.Api.Sdk` | Client-facing SDK project for API consumers. |

## Core Domain Concepts

| Concept | Description |
| --- | --- |
| Manufacturer | Organization or entity associated with product creation. Manufacturers can own multiple products. |
| Product | Conceptual item, such as `T-Shirt`. Products can have multiple variants and can be soft deleted while retaining history. |
| Variant | Smallest stockable and sellable unit, such as `T-Shirt / Red / Medium`. Inventory is tracked at this level. |
| Inventory movement | Immutable stock change for a variant. Quantity controls increase or decrease; type gives business meaning. |
| Delivery | Grouping of stock intake movements for traceability of received stock. |

Movement types are `DELIVERY`, `SALE`, `RETURN`, `LOSS`, and `ADJUSTMENT`. Movement origins are `INTERNAL` for staff-created changes and `EXTERNAL` for changes received from other services.

## Technology Stack

- .NET 10
- ASP.NET Core
- Emit mediator, Kafka, MongoDB, health checks, and OpenTelemetry integrations
- MongoDB
- Kafka and schema registry
- Apache Avro event schemas
- Keycloak-compatible JWT bearer authentication
- OpenTelemetry, Prometheus metrics, and OTLP export
- Docker and Kubernetes deployment target

## Configuration

Configuration is grouped by concern in options classes and resolved through the options pattern. The API expects these major configuration sections:

- `Database`: MongoDB connection string and database name.
- `Kafka`: broker and schema registry settings.
- `Authentication`: authority, audience, HTTPS metadata behavior, and claim mappings.
- `Observability`: service identity, metrics endpoint, tracing exporter, and logging exporter settings.

Development defaults live in:

- `src/Celeste.Inventory.Api/appsettings.json`
- `src/Celeste.Inventory.Api/appsettings.Development.json`

## Running Locally

Prerequisites:

- .NET 10 SDK
- MongoDB
- Kafka
- Schema registry
- Keycloak or compatible JWT issuer for authenticated requests
- Optional OTLP collector, Prometheus, and Grafana for local observability

Build the solution:

```bash
dotnet build
```

Run the API:

```bash
dotnet run --project src/Celeste.Inventory.Api/Celeste.Inventory.Api.csproj
```

Development endpoints:

- HTTP: `http://localhost:5201`
- HTTPS: `https://localhost:7226`
- Swagger UI: `https://localhost:7226/swagger` or `http://localhost:5201/swagger`
- OpenAPI document: `/openapi/v1.json`

## Health Checks

The API exposes:

- `/health/live`
- `/health/ready`

Liveness is process-only. Readiness checks downstream dependencies required to safely receive traffic. MongoDB readiness reports unhealthy when unavailable, while Kafka readiness can report degraded because the service uses an outbox-style event flow and may tolerate short broker outages.

See `docs/health-checks.md` for more detail.

## Observability

The service is instrumented with OpenTelemetry for metrics, traces, and logs. Local configuration exposes a Prometheus scraping endpoint and exports traces/logs to the configured OTLP endpoint.

The PRD targets:

- Grafana and Prometheus for local observability.
- Azure Application Insights for production observability.

## Testing

Run all tests:

```bash
dotnet test
```

Test projects are organized by layer:

- `tests/Celeste.Inventory.Api.Tests`
- `tests/Celeste.Inventory.Application.Tests`
- `tests/Celeste.Inventory.Common.Tests`
- `tests/Celeste.Inventory.Core.Tests`
- `tests/Celeste.Inventory.Infrastructure.Tests`

## Documentation

Additional documentation lives under `docs/`:

- `docs/prd.md`: product requirements and milestone scope.
- `docs/health-checks.md`: liveness and readiness behavior.
- `docs/emit_usage.md`: Emit usage notes.
- `docs/milestones/`: milestone-specific implementation notes.

## System Boundaries

In scope:

- Catalog management.
- Variant-level inventory tracking.
- Immutable inventory movement history.
- Delivery grouping for stock intake.
- Event-based integration with external systems.

Out of scope:

- Order management.
- Payment processing.
- Shipment tracking.
- Inventory reservation.
- Multi-store or multi-warehouse inventory.
- Advanced inventory features such as batching.

## Deployment

The service is intended to be packaged as a Docker image and deployed to Kubernetes. Environment-specific configuration is expected for development, staging, and production. CI/CD workflow files should be generated by the platform toolchain rather than hand-authored in this repository.
