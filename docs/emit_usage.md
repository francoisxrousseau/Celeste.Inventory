# Emit Usage Reference

This document is the project reference for how to use Emit in this solution.
It is intentionally practical and narrow. Use these patterns unless there is a strong reason not to.

---

## Packages in use

Use the Emit packages already available through NuGet in this solution:

- `Emit.Mediator`
- `Emit.Persistence.MongoDB`
- `Emit.Kafka`
- `Emit.Kafka.AvroSerializer`
- `Emit.Kafka.JsonSerializer`
- `Emit.Kafka.HealthChecks`
- `Emit.OpenTelemetry`

Do not invent alternate libraries or patterns for mediator, Kafka, transactional outbox, or Mongo persistence if Emit already covers the use case.

---

## Core rules for this project

- Use `AddEmit(...)` as the root registration point.
- Register mediator handlers explicitly.
- Register Kafka topics explicitly.
- Register MongoDB explicitly and enable the outbox there.
- Prefer Emit transactional handling for mediator handlers and Kafka consumers that both write data and publish events.
- Keep topic names and Kafka client settings in configuration or integration code, not in domain logic.
- Use one handler per request type.
- Use the outbox by default for producer messages unless there is a clear reason to bypass it.

---

## Service installation pattern

Use one central registration point in startup / composition root.

```csharp
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(databaseOptions.ConnectionString));

builder.Services.AddEmit(emit =>
{
    emit.AddMediator(mediator =>
    {
        mediator.AddHandler<CreateManufacturerHandler>();
        mediator.AddHandler<UpdateManufacturerHandler>();
        mediator.AddHandler<DeleteManufacturerHandler>();
        mediator.AddHandler<GetManufacturerByIdHandler>();
        mediator.AddHandler<SearchManufacturersHandler>();
    });

    emit.AddMongoDb(mongo =>
    {
        mongo.Configure((sp, ctx) =>
        {
            ctx.Client = sp.GetRequiredService<IMongoClient>();
            ctx.Database = ctx.Client.GetDatabase(databaseOptions.Name);
        })
        .UseOutbox()
        .UseDistributedLock();
    });

    emit.AddKafka(kafka =>
    {
        kafka.ConfigureClient(cfg =>
        {
            cfg.BootstrapServers = kafkaOptions.BootstrapServers;
        });

        kafka.Topic<string, ManufacturerEvent>("celeste.inventory.manufacturer", topic =>
        {
            topic.SetKeySerializer(Confluent.Kafka.Serializers.Utf8);
            topic.SetValueSerializer(manufacturerEventSerializer);
            topic.SetKeyDeserializer(Confluent.Kafka.Deserializers.Utf8);
            topic.SetValueDeserializer(manufacturerEventDeserializer);

            topic.Producer();

            topic.ConsumerGroup("celeste.inventory.manufacturer", group =>
            {
                group.AddConsumer<ManufacturerEventConsumer>();
            });
        });
    });
});
```

Notes:

- `ConfigureClient(...)` is required for Kafka.
- `mongo.Configure(...)` is required for Mongo.
- `UseOutbox()` enables the transactional outbox infrastructure.
- `UseDistributedLock()` enables distributed locking support for Emit daemons such as the outbox worker.

---

## Mediator

Emit mediator dispatches typed in-process requests to registered handlers.

### Request pattern

```csharp
public sealed record CreateManufacturerCommand(
    string Name,
    string? ContactEmail,
    string? ContactPhone,
    string? User) : IRequest<ManufacturerDto>;

public sealed record GetManufacturerByIdQuery(
    Guid Id,
    bool AllowDeleted) : IRequest<ManufacturerDto?>;
```

### Handler pattern

```csharp
[Transactional]
public sealed class CreateManufacturerHandler(
    IManufacturerRepository repository,
    IEventProducer<string, ManufacturerEvent> producer)
    : IRequestHandler<CreateManufacturerCommand, ManufacturerDto>
{
    public async Task<ManufacturerDto> HandleAsync(
        CreateManufacturerCommand request,
        CancellationToken cancellationToken)
    {
        var manufacturer = new Manufacturer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            CreatedBy = request.User ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        await repository.CreateAsync(manufacturer, cancellationToken);

        await producer.ProduceAsync(
            manufacturer.Id.ToString(),
            ManufacturerEventFactory.Created(manufacturer, request.User),
            cancellationToken);

        return new ManufacturerDto(
            manufacturer.Id,
            manufacturer.Name,
            manufacturer.ContactEmail,
            manufacturer.ContactPhone);
    }
}
```

### Dispatching pattern

```csharp
public sealed class ManufacturerAppService(IMediator mediator)
{
    public Task<ManufacturerDto> CreateAsync(CreateManufacturerCommand command, CancellationToken ct)
        => mediator.SendAsync<ManufacturerDto>(command, ct);
}
```

Rules:

- Handlers are resolved from DI.
- There must be exactly one handler per request type.
- Put write orchestration in command handlers.
- Put read orchestration in query handlers.
- Prefer `[Transactional]` for handlers that write business data and produce events.

---

## MongoDB persistence

Emit MongoDB support provides the outbox repository and distributed lock provider. You still provide the Mongo client and database.

### Registration pattern

```csharp
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(databaseOptions.ConnectionString));

builder.Services.AddEmit(emit =>
{
    emit.AddMongoDb(mongo =>
    {
        mongo.Configure((sp, ctx) =>
        {
            ctx.Client = sp.GetRequiredService<IMongoClient>();
            ctx.Database = ctx.Client.GetDatabase(databaseOptions.Name);
        })
        .UseOutbox(options =>
        {
            options.PollingInterval = TimeSpan.FromSeconds(5);
            options.BatchSize = 100;
        })
        .UseDistributedLock();
    });
});
```

### Repository pattern

```csharp
public sealed class ManufacturerRepository : IManufacturerRepository
{
    private readonly IMongoCollection<ManufacturerDocument> _collection;

    public ManufacturerRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<ManufacturerDocument>("manufacturers");
    }

    public Task CreateAsync(Manufacturer entity, CancellationToken ct)
        => _collection.InsertOneAsync(ManufacturerDocument.FromDomain(entity), cancellationToken: ct);

    public Task<ManufacturerDocument?> GetByIdAsync(Guid id, CancellationToken ct)
        => _collection.Find(x => x.Id == id && x.DeletedAt == null)
            .FirstOrDefaultAsync(ct);
}
```

Project guidance:

- Use a simple feature-specific repository such as `IManufacturerRepository` / `ManufacturerRepository`.
- Do not introduce a generic repository base unless there is a real, current need.
- Normalize values before database requests when the feature requires normalized search or uniqueness.

---

## Transactional outbox

Emit’s outbox solves the dual-write problem by writing business data and the outbound message enqueue in the same database transaction.

Use the outbox by default.

### How it should work here

- Handler writes manufacturer data.
- Handler calls `ProduceAsync(...)`.
- Both the business write and the queued outbox message commit together.
- A background outbox worker later delivers the message to Kafka.
- If delivery fails, the outbox entry remains and is retried.

### Default usage

```csharp
emit.AddMongoDb(mongo =>
{
    mongo.Configure((sp, ctx) =>
    {
        ctx.Client = sp.GetRequiredService<IMongoClient>();
        ctx.Database = ctx.Client.GetDatabase(databaseOptions.Name);
    })
    .UseOutbox();
});

emit.AddKafka(kafka =>
{
    kafka.ConfigureClient(cfg => cfg.BootstrapServers = kafkaOptions.BootstrapServers);

    kafka.Topic<string, ManufacturerEvent>("celeste.inventory.manufacturer", topic =>
    {
        topic.Producer();
    });
});
```

### Bypassing the outbox

Only bypass the outbox for messages that do not need transactional guarantees.

```csharp
kafka.Topic<string, AnalyticsEvent>("celeste.analytics", topic =>
{
    topic.Producer(p => p.UseDirect());
});
```

Project rule:

- Manufacturer create/update/delete events should use the outbox, not direct mode.

---

## Transactions

Use Emit transaction support instead of hand-rolled transaction orchestration.

### Preferred option for handlers and consumers

Use `[Transactional]` on mediator handlers and Kafka consumers that both write data and emit events.

```csharp
[Transactional]
public sealed class UpdateManufacturerHandler(
    IManufacturerRepository repository,
    IEventProducer<string, ManufacturerEvent> producer)
    : IRequestHandler<UpdateManufacturerCommand, ManufacturerDto>
{
    public async Task<ManufacturerDto> HandleAsync(UpdateManufacturerCommand request, CancellationToken ct)
    {
        var manufacturer = await repository.GetRequiredAsync(request.Id, ct);

        manufacturer.Name = request.Name;
        manufacturer.ContactEmail = request.ContactEmail;
        manufacturer.ContactPhone = request.ContactPhone;
        manufacturer.LastUpdatedBy = request.User ?? string.Empty;
        manufacturer.LastUpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(manufacturer, ct);

        await producer.ProduceAsync(
            manufacturer.Id.ToString(),
            ManufacturerEventFactory.Updated(manufacturer, request.User),
            ct);

        return new ManufacturerDto(
            manufacturer.Id,
            manufacturer.Name,
            manufacturer.ContactEmail,
            manufacturer.ContactPhone);
    }
}
```

### Explicit unit of work

Use `IUnitOfWork` only when work happens outside normal mediator / consumer pipelines or when you need explicit commit control.

```csharp
public sealed class ManufacturerRebuildJob(
    IUnitOfWork unitOfWork,
    IEventProducer<string, ManufacturerEvent> producer)
{
    public async Task RunAsync(CancellationToken ct)
    {
        await using var tx = await unitOfWork.BeginAsync(ct);

        // business writes
        // producer calls

        await tx.CommitAsync(ct);
    }
}
```

Project rule:

- For normal API and consumer flows, prefer `[Transactional]`.
- Do not add manual transaction code unless there is a concrete need.

---

## Kafka setup

All Kafka configuration flows through `AddKafka(...)`.

### Minimal topic registration pattern

```csharp
emit.AddKafka(kafka =>
{
    kafka.ConfigureClient(cfg =>
    {
        cfg.BootstrapServers = kafkaOptions.BootstrapServers;
    });

    kafka.Topic<string, ManufacturerEvent>("celeste.inventory.manufacturer", topic =>
    {
        topic.SetKeySerializer(Confluent.Kafka.Serializers.Utf8);
        topic.SetValueSerializer(manufacturerEventSerializer);
        topic.SetKeyDeserializer(Confluent.Kafka.Deserializers.Utf8);
        topic.SetValueDeserializer(manufacturerEventDeserializer);

        topic.Producer();

        topic.ConsumerGroup("celeste.inventory.manufacturer", group =>
        {
            group.AddConsumer<ManufacturerEventConsumer>();
        });
    });
});
```

Rules:

- `ConfigureClient(...)` is required.
- Everything else is per-topic.
- Keep topic names in integration/configuration code.
- Register only the topics this service actually owns or consumes.

---

## Kafka producers

Once `topic.Producer()` is registered, Emit exposes `IEventProducer<TKey, TValue>` from DI.

### Producer pattern

```csharp
public sealed class ManufacturerEventPublisher(
    IEventProducer<string, ManufacturerEvent> producer)
{
    public Task PublishCreatedAsync(ManufacturerEvent evt, CancellationToken ct)
        => producer.ProduceAsync(evt.Id.ToString(), evt, ct);
}
```

### Headers pattern

Use `EventMessage<TKey, TValue>` only when headers are needed.

```csharp
var message = new EventMessage<string, ManufacturerEvent>(
    manufacturer.Id.ToString(),
    evt,
    [new KeyValuePair<string, string>("event-type", evt.EventType)]);

await producer.ProduceAsync(message, ct);
```

Project rule:

- Use the entity id as Kafka key unless there is a strong reason not to.
- Emit manufacturer events only after persistence succeeds.
- Prefer a small factory/helper for event creation and event type names.

---

## Kafka consumers

Use consumers when this service must react to external events.

### Consumer pattern

```csharp
[Transactional]
public sealed class ManufacturerEventConsumer(
    IMediator mediator)
    : IConsumer<UpstreamManufacturerEvent>
{
    public Task ConsumeAsync(ConsumeContext<UpstreamManufacturerEvent> context, CancellationToken ct)
    {
        var message = context.Message;

        return mediator.SendAsync(
            new SyncManufacturerFromEventCommand(
                message.Id,
                message.Name,
                message.ContactEmail,
                message.ContactPhone,
                message.User),
            ct);
    }
}
```

### ConsumeContext usage

`ConsumeContext<T>` gives access to:

- `Message` for the deserialized payload
- `Headers` for Kafka headers
- `RetryAttempt` for retry count
- `TransportContext` for Kafka metadata
- `Transaction` for active transaction context when transactional handling is in use
- `Services` for message-scoped DI access

Project rule:

- Keep consumers thin.
- Translate incoming messages into commands.
- Put business behavior in handlers, not in consumer bodies.

---

## Kafka routing

Use Emit content-based routing only when one Kafka topic carries multiple logical event types and routing by message content is actually needed.

### Router pattern

```csharp
kafka.Topic<string, InventoryIntegrationEvent>("celeste.inventory.integration", topic =>
{
    topic.ConsumerGroup("celeste.inventory.integration", group =>
    {
        group.AddRouter(
            identifier: "inventory-event-router",
            selector: ctx => ctx.Message.EventType,
            configure: router =>
            {
                router.AddRoute("manufacturer.created", r => r.AddConsumer<ManufacturerCreatedConsumer>());
                router.AddRoute("manufacturer.updated", r => r.AddConsumer<ManufacturerUpdatedConsumer>());
                router.AddRoute("manufacturer.deleted", r => r.AddConsumer<ManufacturerDeletedConsumer>());
            });
    });
});
```

Project rule:

- Do not introduce routing if a dedicated topic and single consumer are enough.
- Prefer simple topic + consumer registration unless mixed-event topics are already mandated.

---

## Serialization

Emit delegates Kafka serialization to Confluent serializers and deserializers.

### Serializer registration pattern

```csharp
kafka.Topic<string, ManufacturerEvent>("celeste.inventory.manufacturer", topic =>
{
    topic.SetKeySerializer(Confluent.Kafka.Serializers.Utf8);
    topic.SetValueSerializer(manufacturerEventSerializer);
    topic.SetKeyDeserializer(Confluent.Kafka.Deserializers.Utf8);
    topic.SetValueDeserializer(manufacturerEventDeserializer);

    topic.Producer();
});
```

Rules:

- Set serializers per topic.
- Keep serializer choices consistent for a given topic.
- Use the project’s chosen JSON / Avro serializer implementation instead of inventing custom wire formats.

---

## Recommended project usage for Manufacturer milestone

For the Manufacturer milestone:

- Use Emit mediator for commands and queries.
- Use MongoDB for persistence.
- Enable Mongo outbox.
- Use `[Transactional]` on write handlers.
- Publish create, update, and delete events through `IEventProducer<string, ManufacturerEvent>`.
- Use topic `celeste.inventory.manufacturer`.
- Keep repository implementation simple and feature-specific.
- Keep consumers thin if any are needed in this milestone.
- Normalize manufacturer names before database access.

---

## Anti-patterns to avoid

Do not:

- invent a second mediator abstraction
- add a generic repository framework for milestone one
- hardcode Kafka topic logic inside domain entities
- bypass the outbox for manufacturer state-change events
- put business logic in controllers or consumers
- create multiple handlers for the same request type
- introduce content-based routing unless the topic genuinely contains multiple logical event types

---

## Prompting guidance for Codex

When implementing code, use this document as the reference for Emit usage.

Expected behavior:

- follow the registration patterns in this file
- use Emit.Mediator for application requests
- use Emit.Persistence.MongoDB for Mongo registration and outbox
- use Emit.Kafka for topic registration, producers, and consumers
- use `[Transactional]` for transactional handlers/consumers that write data and publish events
- do not invent alternate integration patterns unless explicitly requested
