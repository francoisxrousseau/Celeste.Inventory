# Health Checks

## Endpoints

- `/health/live`
- `/health/ready`

## Behavior

- Liveness (`/health/live`) is process-only and reports whether the API is running.
- Readiness (`/health/ready`) includes downstream dependency checks needed to safely receive traffic.

## Dependency checks in readiness

- `AddEmitKafka()` is registered with readiness tags.
- `AddEmitMongoDB()` is registered with readiness tags.

Both checks are read-only and lightweight, following Emit provider health checks.

## Failure semantics

- Kafka readiness uses `Degraded` because this service uses an outbox flow and can often continue to accept and persist work during short broker outages.
- MongoDB readiness uses `Unhealthy` because API request handling depends on database connectivity.

## Why dependencies are not in liveness

Dependency outages should not trigger container restarts if the process itself is healthy. Keeping liveness process-only avoids restart loops during transient Kafka or MongoDB issues and aligns with Kubernetes probe best practices.
