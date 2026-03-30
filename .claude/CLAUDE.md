# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build all projects
dotnet build

# Run the API (HTTP: 5201, HTTPS: 7226)
dotnet run --project src/Celeste.Inventory.Api/Celeste.Inventory.Api.csproj
```

## Architecture

This is a .NET 10 inventory microservice within the Celeste Labs platform, following a layered clean architecture.

### Project Structure

| Project | Role |
|---|---|
| `Celeste.Inventory.Api` | ASP.NET Core Web API — controllers, validators, messaging, DI installers |
| `Celeste.Inventory.Application` | CQRS handlers, SignalR hubs, mapping, DI installers |
| `Celeste.Inventory.Core` | Domain entities, repository interfaces, options — pure domain concerns |
| `Celeste.Inventory.Infrastructure` | Repository implementations, document models, DB mapping |
| `Celeste.Inventory.Common` | Cross-cutting DTOs, enums, request parameters, response models |
| `Celeste.Inventory.Events` | Kafka event declarations published by Application handlers via a Kafka Outbox pattern (third-party library) |
| `Celeste.Inventory.Api.Sdk` | Client library for API consumers — exposes SignalR hub client setup and authentication methods (same auth as the API) |

### Request Flow

```
Api (Controller)
  → sends Command/Query via CQRS
    → Application (Handler)
      → calls Core repository interface
        → Infrastructure (Repository implementation)
      → publishes Kafka event (Events project) via Outbox
```

### Conventions

- **DI registration** lives in `Installers/` folders within each project — new services/repos should be registered there.
- **Documentation**: All public types and members must have XML doc comments (`/// <summary>`). Keep summaries to 2–3 lines maximum.
- **Options pattern**: All configuration must be declared in a dedicated options file and grouped into a focused options class per concern. Resolve options exclusively via `IOptions<T>` (or `IOptionsSnapshot<T>` / `IOptionsMonitor<T>` where appropriate) — never bind config directly.
- **`.claude/rules/`** may contain project-specific rules — check there for additional guidance.
- **Async code**: Whenever possible, use async overload of methods, use CancellationToken with default value overload.
## Deployment

Dockerfiles, Kubernetes manifests, and GitHub Actions workflows are not present yet but are planned:

- The service will be packaged as a **Docker image** and deployed to a **Kubernetes cluster** using manifests.
- **Multi-environment** support (e.g. dev, staging, prod) will be handled through environment-specific configuration.
- **CI/CD pipelines** (PR checks and merge-to-main) will be generated via a CICD toolchain — do not hand-author GitHub workflow files.
