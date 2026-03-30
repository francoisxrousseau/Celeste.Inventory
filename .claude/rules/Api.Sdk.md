# Celeste.Inventory.Api.Sdk.md
project:
  name: API.SDK
  description: |
    This project implements the SDK client layer for consuming the API project.
    It provides a strongly-typed client for all controllers, exposing HTTP communication,
    authentication, and real-time connections in a consumable NuGet package.

folder_structure:
  layout:
    Installers/:
      purpose: |
        Extension methods for registering the SDK in a consumer's DI container.
        This is the entry point for any project that references the SDK NuGet package.
    Options/:
      purpose: |
        SDK configuration options classes (base address, authentication settings, etc.).
        Must follow the Options pattern — resolved via IOptions<T>, never bound directly.

client:
  http_client:
    description: |
      The SDK must encapsulate an HTTP client using IHttpClientFactory with a named client.
      All requests to the API controllers must go through this client.
    configuration:
      options:
        description: |
          The API base address must be configurable via a dedicated options class in Options/.
          Consumers specify the HTTP endpoint through the options system — never hardcoded.

authentication:
  description: |
    The SDK must allow specifying a bearer token for accessing the API.
    The token must be injected into all outgoing HTTP requests automatically.
    Token configuration must be encapsulated in an options class — consumers must not
    manually set request headers.

real_time:
  description: |
    The SDK must expose methods to connect and register to the SignalR hub(s) provided by the API.
    SignalR connection setup and hub registration must be encapsulated and reusable across consumers.
  notes: |
    Authentication used for SignalR connections must match the same token mechanism
    used for HTTP requests.

installer:
  description: |
    The SDK must provide DI installer extension methods (e.g. AddInventorySdk()) that register
    the HTTP client, SignalR setup, and all SDK services in a single call.
    These installers must work in any consumer project referencing the SDK NuGet package.

general_guidelines:
  - The SDK must not contain business logic; it only wraps API interactions.
  - All communication details (HTTP, headers, tokens, SignalR) must be encapsulated inside the SDK.
  - Configuration, instantiation, and DI registration must be handled via installer methods.
