# Event Sourcing

## Purpose

This document describes the event sourcing approach used in this reference architecture.

The goal is to demonstrate how a cloud-native distributed system can use immutable domain events as the primary system of record, while deriving query-optimised read models through asynchronous projection processing.

---

# Summary

Event sourcing stores the history of business changes as a sequence of immutable events.

Instead of storing only the current state of an entity, the system stores facts about what happened.

Example:

```text
BlastPlanCreated
DrillPatternDesigned
BlastApproved
BlastFired
BlastReconciled
```

The current state of a domain object is reconstructed by replaying its event stream.

---

# Design Rationale

Event sourcing is used because the reference domain benefits from:

- auditability,
- traceability,
- temporal reconstruction,
- workflow history,
- replayable projections,
- and explicit domain behaviour.

Industrial operational systems often need to answer not only:

```text
What is the current state?
```

but also:

```text
How did the system reach this state?
Who changed it?
When did it happen?
What downstream views were derived from it?
Can the state be reconstructed?
```

Event sourcing directly supports these concerns.

---

# Core Concepts

## Event

An event is an immutable fact that records something meaningful that has already happened in the domain.

Examples:

```text
BlastPlanCreated
HoleDesigned
BlastPlanApproved
BlastFired
```

Events are named in the past tense because they represent completed facts.

Events should be:

- immutable,
- append-only,
- business meaningful,
- versioned,
- and self-describing enough to support future processing.

---

## Aggregate

An aggregate protects business invariants and produces events.

The aggregate does not directly persist state. Instead, it:

1. receives a command,
2. validates business rules,
3. emits one or more domain events,
4. applies those events to update in-memory state.

Example:

```text
ApproveBlastPlan command
  -> BlastPlan aggregate validates approval rules
  -> BlastPlanApproved event emitted
```

---

## Event Stream

An event stream is an ordered sequence of events for a specific aggregate instance.

Example stream:

```text
stream: blast-plan-4f3c2a

1. BlastPlanCreated
2. HoleDesigned
3. HoleDesigned
4. BlastPlanApproved
5. BlastFired
```

The stream represents the authoritative history of that aggregate.

---

## Event Store

The event store is the system of record.

In this project, Azure Cosmos DB is used as the event store.

Responsibilities:

- persist immutable events,
- maintain event order per aggregate stream,
- enforce optimistic concurrency,
- support aggregate rehydration,
- support projection replay.

---

## Projection

A projection consumes events and builds a query-optimised read model.

In this project, Azure SQL Database is used for projections.

Example:

```text
BlastPlanCreated
HoleDesigned
BlastPlanApproved
  -> BlastPlanSummaryProjection
  -> Azure SQL
```

Read models are derived data. They can be rebuilt from the event store.

---

# Write Model Flow

The write side is responsible for handling commands and persisting domain events.

```text
Client
  -> API Endpoint
  -> Command Handler
  -> Aggregate
  -> Event Store
```

Detailed flow:

1. API receives a command.
2. Application layer validates command shape.
3. Command handler loads the aggregate event stream.
4. Aggregate is rehydrated from historical events.
5. Command is executed against the aggregate.
6. Aggregate emits new domain events.
7. Events are appended to Cosmos DB using optimistic concurrency.
8. Events are published for projection processing.

---

# Read Model Flow

The read side is responsible for query workloads.

```text
Event Store
  -> Projection Processor
  -> Azure SQL Read Model
  -> Query API
  -> Client
```

Detailed flow:

1. Domain events are persisted to Cosmos DB.
2. Projection processor receives or polls new events.
3. Events are applied to projection handlers.
4. Projection handlers update SQL read models.
5. Query API reads from SQL.
6. Client receives query-optimised data.

---

# Consistency Model

This architecture uses eventual consistency between the write model and read model.

After a command succeeds, the event store is immediately consistent for that aggregate stream.

The SQL read model may lag behind while projections are processed.

This is intentional.

```text
Command accepted
  -> Event persisted
  -> Projection updated asynchronously
  -> Query model eventually reflects the change
```

Consumers must tolerate projection lag.

---

# Optimistic Concurrency

Optimistic concurrency is used to prevent conflicting writes to the same aggregate stream.

Each append operation includes the expected stream version.

Example:

```text
Current stream version: 4
Command expects version: 4
Append event as version: 5
```

If another write has already appended version 5, the append fails with a concurrency conflict.

The command can then be retried or rejected depending on business semantics.

---

# Event Store Design

## Cosmos DB Container

A simplified Cosmos DB event document may look like:

```json
{
  "id": "blast-plan-4f3c2a-00000005",
  "streamId": "blast-plan-4f3c2a",
  "aggregateId": "4f3c2a",
  "aggregateType": "BlastPlan",
  "sequence": 5,
  "eventType": "BlastPlanApproved",
  "eventVersion": 1,
  "occurredUtc": "2026-05-26T01:15:00Z",
  "correlationId": "c9e97c91a4b54a04b8f1dfc3bb51b8ef",
  "causationId": "f5fa57a5cefb4eaf91e2302c3ef7c9c2",
  "metadata": {
    "userId": "user-123",
    "tenantId": "tenant-abc",
    "source": "BlastPlanning.Api"
  },
  "data": {
    "blastPlanId": "4f3c2a",
    "approvedBy": "user-123",
    "approvedUtc": "2026-05-26T01:15:00Z"
  }
}
```

## Recommended Partitioning

Recommended logical partition key:

```text
streamId
```

This keeps all events for a single aggregate stream in the same logical partition.

Benefits:

- efficient stream reads,
- ordered aggregate reconstruction,
- simpler optimistic concurrency,
- predictable aggregate-level consistency.

Trade-off:

- hot streams must be avoided through appropriate aggregate boundaries.

---

# Event Publishing

Persisting an event and publishing it to downstream processors must be reliable.

Preferred pattern:

```text
Append events to event store
Append outbox messages in same logical operation
Background dispatcher publishes messages
Projection processors consume messages
```

This avoids losing events between persistence and publication.

In this project, the outbox pattern should be used before introducing direct broker publication from command handlers.

---

# Projection Design

## SQL Read Models

Projection tables are query-optimised and intentionally denormalised.

Example table:

```text
BlastPlanSummary
  BlastPlanId
  Name
  SiteId
  Status
  HoleCount
  ApprovedBy
  ApprovedUtc
  FiredUtc
  LastUpdatedUtc
```

The projection is not the source of truth.

It can be deleted and rebuilt by replaying events from Cosmos DB.

---

## Idempotency

Projection handlers must be idempotent.

A projection may receive the same event more than once due to retry behaviour.

Each projection should track processed event identity.

Example:

```text
ProcessedProjectionEvents
  ProjectionName
  EventId
  ProcessedUtc
```

Before applying an event, the projection handler checks whether the event has already been processed.

---

## Replay

Replay is the process of rebuilding projections from historical events.

Replay is useful for:

- creating new read models,
- fixing projection bugs,
- rebuilding corrupted read models,
- supporting schema evolution.

Replay must be controlled to avoid disrupting live query workloads.

Recommended approach:

```text
Create new projection table
Replay historical events into new table
Validate row counts and checksums
Swap read model version
Retire old projection
```

---

# Event Versioning

Events are immutable. Once published, an event contract should not be changed destructively.

Acceptable changes:

- adding optional fields,
- introducing a new event version,
- adding new event types.

Avoid:

- renaming existing fields,
- changing field meaning,
- deleting fields,
- changing event semantics.

Recommended strategy:

```text
BlastPlanApproved.v1
BlastPlanApproved.v2
```

or:

```json
{
  "eventType": "BlastPlanApproved",
  "eventVersion": 2
}
```

Projection handlers should explicitly handle supported versions.

---

# Snapshots

Snapshots may be introduced for aggregates with long event streams.

A snapshot captures aggregate state at a point in time.

Example:

```text
Load snapshot at version 500
Replay events 501-530
Execute command
Append event 531
```

Snapshots are an optimisation only.

The event stream remains the source of truth.

Snapshots should not be introduced until stream replay performance requires them.

---

# Error Handling

## Command Failures

Command failures should be explicit.

Examples:

- validation failure,
- aggregate not found,
- business rule violation,
- concurrency conflict.

Command failures should not produce domain events unless the failure itself is a meaningful business fact.

---

## Projection Failures

Projection failures should be recoverable.

Recommended behaviour:

- retry transient failures,
- dead-letter poison events,
- alert on repeated failures,
- allow projection replay after correction.

Projection failures must not corrupt the event store.

---

# Correlation and Causation

Every event should include:

- `correlationId`
- `causationId`

## Correlation ID

Groups all events and operations belonging to the same logical workflow.

## Causation ID

Identifies the command, message, or event that caused the current event.

These identifiers support:

- distributed tracing,
- debugging,
- auditability,
- incident investigation.

---

# Testing Strategy

Event-sourced systems are well suited to scenario-based tests.

Example test structure:

```text
Given:
  BlastPlanCreated
  HoleDesigned

When:
  ApproveBlastPlan

Then:
  BlastPlanApproved
```

Tests should focus on domain behaviour rather than implementation details.

Recommended test categories:

- aggregate behaviour tests,
- command handler tests,
- event store contract tests,
- projection handler tests,
- replay tests,
- concurrency tests.

---

# Trade-Offs

## Benefits

- complete audit trail,
- strong domain traceability,
- replayable projections,
- temporal debugging,
- explicit business history,
- improved integration flexibility.

## Costs

- higher implementation complexity,
- eventual consistency,
- event schema evolution,
- projection lag,
- replay operational complexity,
- more demanding testing discipline.

Event sourcing should be used deliberately where the audit and workflow benefits justify the additional complexity.

---

# Project Position

This project uses event sourcing for the core operational workflow because the domain benefits from:

- auditable state transitions,
- complex lifecycle management,
- historical reconstruction,
- and asynchronous reporting.

It does not imply that every service or entity in the platform should be event-sourced.

The preferred approach is selective event sourcing for high-value aggregates and conventional persistence for simpler data management needs.