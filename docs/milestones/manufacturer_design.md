\# Manufacturer Feature Specification



\## Overview



The Manufacturer feature allows administrators to manage manufacturers associated with products in the catalog.



Manufacturers are simple entities used for organizational and reference purposes. They can be created, updated, retrieved, listed, and soft deleted.



\---



\## Domain Model



\### Manufacturer



Represents a manufacturer associated with products.



Fields:

\- Id (Guid): Unique identifier

\- Name (string): Display name of the manufacturer (required)

\- ContactEmail (string, optional)

\- ContactPhone (string, optional)

\- Audit fields inherited from AuditableEntity:

&#x20; - CreatedBy

&#x20; - CreatedAt

&#x20; - LastUpdatedBy

&#x20; - LastUpdatedAt

&#x20; - DeletedBy

&#x20; - DeletedAt

&#x20; - IsDeleted



Notes:

\- A manufacturer is considered inactive if `IsDeleted = true`

\- No hard deletes are allowed



\---



\## Behavioral Specification



\### Create Manufacturer

\- A manufacturer is created with a unique identifier

\- Name is required

\- Contact fields are optional



\### Update Manufacturer

\- Updates existing manufacturer fields

\- Manufacturer must exist

\- Name remains required



\### Delete Manufacturer (Soft Delete)

\- Manufacturer is soft deleted

\- It remains in the system for historical reference

\- No physical deletion occurs



\### Get Manufacturer by Id

\- Returns manufacturer if it exists

\- Returns not found if it does not exist

\- Excludes soft-deleted manufacturers by default

\- Can optionally include deleted manufacturers when explicitly requested



\### List / Search Manufacturers

\- Returns paginated list of manufacturers

\- Supports free-text search on name

\- Excludes soft-deleted manufacturers by default

\- Can optionally include deleted manufacturers

\- Supports count-only queries



\---



\## API Surface



\### Create

POST /manufacturer



\### Update

PUT /manufacturer/{id}



\### Get by Id

GET /manufacturer/{id}



Optional query parameter:

\- allowDeleted (bool, default: false)



\---



\### List / Search

GET /manufacturer



Query parameters:

\- pageNumber (int, default: 1)

\- pageSize (int, default: 25)

\- searchText (string, optional)

\- includeDeleted (bool, default: false)

\- countOnly (bool, default: false)



\---



\### Delete (Soft Delete)

DELETE /manufacturer/{id}



\---



\## Request Models



\### ManufacturerModel



Represents data used for create and update operations.



Fields:

\- Id (Guid, optional for create, required for update)

\- Name (string, required)

\- ContactEmail (string, optional)

\- ContactPhone (string, optional)



\---



\## Acceptance Criteria



\- Manufacturer can be created with valid data

\- Manufacturer name is required

\- Manufacturer can be updated

\- Manufacturer can be soft deleted

\- Deleted manufacturers are excluded from default queries

\- Manufacturer retrieval by Id works correctly

\- allowDeleted parameter allows retrieval of soft-deleted manufacturers

\- Pagination and search behave correctly

\- Count-only queries return correct totals

\- Invalid operations are handled safely



\---



\## Edge Cases



\- Creating a manufacturer with empty or null name → rejected

\- Updating a non-existent manufacturer → error

\- Deleting an already deleted manufacturer → safe no-op

\- Retrieving a non-existent manufacturer → not found

\- Retrieving a deleted manufacturer without allowDeleted → not found

\- Searching with no results → returns empty list

\- Pagination beyond range → returns empty result set



\---



\## Constraints



\- No hard deletes allowed

\- Must follow existing architecture (Domain, Application, Infrastructure, API separation)

\- No business logic in API layer

\- Must respect soft delete behavior across all queries



\---



\## Out of Scope



\- Product management

\- Inventory management

\- External integrations

\- Advanced validation rules

\- Manufacturer hierarchy or grouping

