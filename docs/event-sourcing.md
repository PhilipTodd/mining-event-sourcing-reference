---
layout: default
title: Event Sourcing
permalink: /event-sourcing/
---

# Event Sourcing

## Overview

Traditional applications persist only the current state of an entity. In contrast, **Event Sourcing** persists every change as an immutable sequence of domain events.

The current state of an aggregate is reconstructed by replaying its event history.

This reference implementation uses Event Sourcing to provide a complete audit trail, enable temporal analysis, and support scalable event-driven processing.

---

## Why Event Sourcing?

Event Sourcing provides several benefits over traditional CRUD-based persistence.

### Complete Audit History

Every business decision is permanently recorded.

Rather than simply storing the latest state, the system records **how that state was reached**.

For example:

```text
BlastPlanCreated
BlastPatternUpdated
ChargeDesignCompleted
BlastApproved
BlastExecuted
```

Each event becomes part of the permanent business history.

---

### Immutable Data

Events are never modified.

Corrections are represented by new events rather than updates to existing records.

This ensures the integrity and traceability of historical business activity.

---

### Temporal Queries

Since historical events are preserved, it becomes possible to answer questions such as:

- What did this Blast Plan look like yesterday?
- When was approval granted?
- Which engineer approved the blast?
- How many revisions occurred before execution?

---

### Event Replay

The complete state of an aggregate can be rebuilt by replaying its event stream.

This capability enables:

- rebuilding read models
- repairing projections
- introducing new projections
- debugging historical behaviour

---

## Event Lifecycle

A typical command follows the sequence below.

```text
Command
    ?
    ?
Command Handler
    ?
    ?
Aggregate
    ?
Business Rules
    ?
    ?
Domain Events
    ?
    ?
Cosmos DB Event Store
    ?
    ?
Azure Event Hubs
    ?
    ?
Projection Workers
    ?
    ?
Azure SQL Read Models
```

---

## Event Store

The Event Store is implemented using **Azure Cosmos DB**.

Each aggregate maintains an ordered stream of immutable events.

Example:

```text
BlastPlan
?
??? Event 1
??? Event 2
??? Event 3
??? Event 4
??? Event 5
```

No event is ever updated or deleted.

The current aggregate state is reconstructed by replaying these events in order.

---

## Aggregate Reconstruction

When processing a command:

1. Load the aggregate's event stream.
2. Replay all events.
3. Reconstruct the current aggregate state.
4. Execute business rules.
5. Produce new domain events.
6. Persist new events.

```text
Load Events
      ?
      ?
Replay Events
      ?
      ?
Current Aggregate
      ?
      ?
Execute Command
      ?
      ?
New Events
```

---

## Optimistic Concurrency

Multiple users may attempt to update the same aggregate simultaneously.

The Event Store uses optimistic concurrency to ensure that new events are only appended if the aggregate version has not changed.

If another command has already committed additional events, the command must be retried using the latest aggregate state.

This prevents lost updates without requiring long-running database locks.

---

## Event Versioning

As systems evolve, event schemas inevitably change.

Although this reference implementation currently uses a single event version, production systems commonly support:

- event schema evolution
- backward compatibility
- event up-conversion
- version-aware projections

Support for event versioning is planned as a future enhancement.

---

## Event Replay

One of the major advantages of Event Sourcing is the ability to rebuild read models.

```text
Event Store
      ?
Replay Events
      ?
      ?
Projection Worker
      ?
      ?
Azure SQL
```

Replay allows new projections to be introduced without changing historical data.

---

## Benefits

This architecture provides:

- Complete audit history
- Immutable business records
- High traceability
- Event replay
- Temporal analysis
- Independent read models
- Loose coupling
- Excellent support for asynchronous processing

---

## Trade-offs

Event Sourcing also introduces additional complexity.

### Advantages

- Full business history
- Excellent audit capability
- Event replay
- Natural integration with messaging
- Scalable read models

### Challenges

- Increased implementation complexity
- Event schema evolution
- Eventual consistency
- Additional infrastructure
- More sophisticated debugging

For systems requiring auditability and historical traceability, these trade-offs are often worthwhile.

---

## Implementation in this Project

This reference implementation demonstrates:

- Domain-driven aggregates
- Immutable domain events
- Azure Cosmos DB Event Store
- Azure Event Hubs integration
- Asynchronous projections
- Azure SQL read models
- Optimistic concurrency
- Replayable event streams

Future iterations will introduce:

- Snapshotting
- Event versioning
- Outbox Pattern
- Inbox Pattern
- Saga orchestration

---

## Related Documentation

- [Architecture](/architecture/)
- [Technology](/technology/)
- [CQRS](/cqrs/)
- [Deployment](/deployment/)