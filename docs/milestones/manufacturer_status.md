# Manufacturer Status

## Completed
- Domain layer (entity, rules, tests)
- Application layer (commands, queries, handlers)
- Infrastructure layer (repository, mapping, Mongo integration)

## Decisions
- Name: trimmed, culture-invariant, case-insensitive uniqueness
- Soft delete only
- Audit: UTC timestamps, user string nullable until auth
- Search: case-insensitive substring
- Kafka topic: celeste.inventory.manufacturer

## Next Step
API layer:
- controller
- validation
- request/response binding

## Constraints
- use TDD skill
- keep controllers thin
- no business logic in API
- no infra logic leakage