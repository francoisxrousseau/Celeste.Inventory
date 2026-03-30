# Celeste.Inventory.Infrastructure.md
project:
  name: Infrastructure
  description: |
    This project implements the infrastructure layer and interacts with a document-oriented database.
    It provides repository implementations and database access for the Application layer.
    It must not expose any database internals outside its own boundaries.

folder_structure:
  layout:
    Repositories/:
      purpose: |
        Concrete implementations of repository interfaces defined in Core/Repositories/.
        Every repository class must implement its corresponding Core interface.
    Documents/:
      purpose: |
        Internal database document models. These classes represent the database schema
        and must never be returned outside this project.
      naming: All document classes must be suffixed with 'Document' (e.g. InventoryItemDocument).
    Mapping/:
      purpose: |
        All mapping logic between database documents and domain objects.
        Mapping must be implemented as dedicated static extension methods or mapper classes here —
        never inline inside a repository method.
    Installers/:
      purpose: DI registration for all repositories and infrastructure services.

database:
  type: document
  description: |
    Repository methods must return domain objects, not database document objects.
    Database documents are internal to this project and must never leak outside.
    All documents must be mapped to domain objects in Mapping/ before being returned.

documents:
  naming_convention:
    description: |
      Every database document class must be suffixed with 'Document'.
      This suffix is the enforced convention — not optional.

operations:
  async:
    description: |
      All database operations must be implemented as asynchronous methods.
      All methods must expose a CancellationToken parameter.
  get_all:
    description: |
      Methods that return collections must support pagination and filtering where applicable,
      consistent with the parameters defined in Celeste.Inventory.Common/Parameters/.

transactions:
  description: |
    All database changes must be executed in a transactional context.
    The Application layer may access the transaction via an interface defined in Core —
    without direct access to database implementation details.

installers:
  description: |
    All DI registrations for this project (repositories, database clients, etc.) must be
    declared in the Installers/ folder and called from Program.cs in the Api project.

general_guidelines:
  - Database document objects must never be exposed outside this project.
  - All document-to-domain mapping must occur in the Mapping/ folder.
  - Business logic must not reside here — Infrastructure is responsible for data access only.
