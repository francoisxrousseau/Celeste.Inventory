# Celeste.Inventory.Events.md
project:
  name: Events
  description: |
    This project contains all event schema definitions for the system.
    Events represent domain processes that have occurred (e.g. ProductCreated, ProductUpdated, ProductDeleted).
    This project must not reference any other internal projects — it is fully independent.
    Any other project that needs to publish or consume events may reference this project.

schemas:
  description: |
    All events must have a clearly defined schema (e.g. Avro) in this project.
    The project must contain only event definitions — no business logic or database access.
  notes: |
    How event classes are generated (e.g. via AvroGen) is implementation guidance,
    not an enforceable rule. The output must still conform to the naming and schema conventions below.

domain_events:
  description: |
    Each event must correspond to a domain action or process that has already occurred.
  naming_convention:
    pattern: "<Entity><PastTenseVerb>" — e.g. ProductCreated, ProductUpdated, ProductDeleted.
    rules:
      - Names must be in past tense to reflect that the action has already happened.
      - Names must clearly identify the entity and the action without ambiguity.

general_guidelines:
  - This project may be referenced by Application or Infrastructure to publish events.
  - No infrastructure, persistence, or business logic is permitted here.
  - The project is a pure schema/contract definition — keep it minimal and stable.
