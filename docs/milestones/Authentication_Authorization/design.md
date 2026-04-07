# Authentication_Authorization – Design

## Overview

This milestone introduces authentication and authorization to the service using Keycloak.

The goal is to make the API work locally and in deployed environments using configuration-driven JWT bearer authentication and scope-based authorization.

This milestone must:
- install authentication and authorization in the API
- add the required options/configuration
- protect the manufacturer endpoints with the correct authorization policies
- expose the authenticated user identity so it can be used for audit fields

This service is a bearer-token API:
- no UI login
- no cookie/session auth
- no interactive authentication flow inside the API

---

## Objectives

- Validate JWT access tokens issued by Keycloak
- Enforce authorization using scopes
- Protect manufacturer endpoints with explicit policies
- Extract the authenticated user id from the JWT `sub` claim
- Keep configuration environment-driven
- Make local development work without hardcoded local-only assumptions
- Make deployment possible by changing configuration only

---

## Keycloak Assumptions

### Realm
- `celeste`

### Authority / Issuer
- `http://localhost:8080/realms/celeste`

### Audience
- `celeste.inventory`

### Required scopes
- `manufacturer.read`
- `manufacturer.write`

### Token requirements
The access token must contain:
- `iss`
- `aud` including `celeste.inventory`
- `scope`
- `sub`
- optionally `preferred_username`

---

## Authentication Design

### Mechanism
- JWT Bearer authentication

### Configuration behavior
Authentication must be fully configuration-driven.

The configuration must support:
- `Authority` → Keycloak issuer URL
- `Audience` → expected API audience (`celeste.inventory`)
- `RequireHttpsMetadata` → controls metadata HTTPS enforcement (environment-dependent)

### Environment behavior
- Local development: HTTPS metadata validation may be disabled
- Deployed environments: HTTPS metadata validation is expected to be enabled

No environment-specific logic must be hardcoded.

---

## Authorization Design

### Strategy
Authorization is scope-based.

### Policies
- `ManufacturerRead` → requires `manufacturer.read`
- `ManufacturerWrite` → requires `manufacturer.write`

### Scope handling requirement
Keycloak may emit scopes as a single space-separated value in a single claim.

Examples:
- `scope = "email profile manufacturer.read"`
- `scope = "manufacturer.read manufacturer.write"`

Authorization must:
- correctly parse space-separated scopes
- not assume one claim per scope
- evaluate scope presence reliably regardless of format

---

## Manufacturer Endpoint Protection

The `ManufacturersController` must be protected with explicit authorization attributes.

The controller must not rely only on a controller-level shared policy because read and write endpoints require different scopes.

### Required protection

| Endpoint | Authorization |
|---|---|
| `GET /manufacturers` | `ManufacturerRead` |
| `GET /manufacturers/{id}` | `ManufacturerRead` |
| `POST /manufacturers` | `ManufacturerWrite` |
| `PUT /manufacturers/{id}` | `ManufacturerWrite` |
| `DELETE /manufacturers/{id}` | `ManufacturerWrite` |

### Required behavior
- a token with `manufacturer.read` must not be allowed to call POST, PUT, or DELETE
- a token without the required scope must receive `403`
- a missing or invalid token must receive `401`

---

## User Identity & Audit

### Source
The authenticated user id must come from the JWT `sub` claim.

### Configuration requirements
The system must support configurable claim mapping:
- `UserIdClaimType` → defaults to `sub`
- `NameClaimType` → defaults to `preferred_username`

### Mapping
- `CreatedBy` → `UserIdClaimType`
- `LastUpdatedBy` → `UserIdClaimType`
- `DeletedBy` → `UserIdClaimType`

### Notes
- the user identifier must be stable and suitable for persistence
- display-oriented claims must not be used for audit
- audit fields remain `string`

---

## Required Components

### Options configuration

A configuration model must exist to bind authentication and identity-related settings.

This configuration must:
- be environment-driven (appsettings, env vars, etc.)
- control authentication behavior
- control claim mapping behavior

No hardcoded values are allowed outside of defaults.