# ADR 0001: Use Event Sourcing for Core Operational Workflows

## Status

Accepted

---

# Context

The platform models operational workflows that benefit from:

- complete auditability,
- historical traceability,
- replayable state reconstruction,
- asynchronous reporting,
- and explicit lifecycle transitions.

The reference domain includes long-running operational processes such as:

- blast planning,
- drill pattern design,
- loading operations,
- approval workflows,
- and blast reconciliation.

These workflows involve multiple state transitions over time and require clear visibility into:

- what changed,
- when it changed,
- who initiated the change,
- and how downstream systems derived their state.

Traditional CRUD-style persistence stores only the latest entity state and does not preserve behavioural history without additional auditing mechanisms.

The project also aims to demonstrate modern distributed systems engineering patterns commonly associated with large-scale cloud-native enterprise platforms.

---

# Decision

The platform will use event sourcing for selected high-value operational aggregates.

Instead of persisting only current entity state, the system will persist immutable domain events representing business facts.

Example events:

```text
BlastPlanCreated
DrillPatternDesigned
BlastPlanApproved
BlastFired
BlastReconciled
```

Aggregate state will be reconstructed by replaying ordered event streams.

The implementation will use:

- Azure Cosmos DB as the event store,
- asynchronous projection processing,
- Azure SQL Database for query-optimised read models,
- CQRS separation between write and read workloads.

The event store will become the authoritative system of record for event-sourced aggregates.

Read models will be considered disposable and rebuildable.

---

# Rationale

Event sourcing was selected because it provides several capabilities that align strongly with the domain and architectural goals of the platform.

## Auditability

Every domain transition is persisted as an immutable historical fact.

This enables full operational traceability without relying on secondary audit tables.

---

## Historical Reconstruction

The system can reconstruct aggregate state at any point in time by replaying historical events.

This supports:

- incident analysis,
- debugging,
- operational investigation,
- and temporal reporting.

---

## Explicit Domain Behaviour

Events model meaningful business behaviour rather than generic data mutations.

Example:

Preferred:

```text
BlastPlanApproved
```

Avoid:

```text
BlastPlanStatusUpdated
```

This encourages clearer domain modelling and stronger ubiquitous language.

---

## Replayable Projections

Read models can be rebuilt from the event history.

This supports:

- new reporting models,
- projection correction,
- schema evolution,
- analytics experimentation.

---

## Asynchronous Scalability

CQRS separation allows independent scaling of:

- command processing,
- projection processing,
- query workloads.

This aligns well with cloud-native distributed systems.

---

## Operational Visibility

Event streams provide strong observability into workflow progression and system behaviour.

This improves:

- debugging,
- telemetry correlation,
- distributed tracing,
- operational transparency.

---

# Consequences

## Positive Consequences

### Strong Audit History

The platform gains complete lifecycle traceability for operational workflows.

---

### Improved Domain Clarity

Business behaviour becomes explicit through event naming and aggregate modelling.

---

### Projection Flexibility

New query models can be introduced without modifying transactional write models.

---

### Replay Capability

Read-side projections can be rebuilt if corrupted or redesigned.

---

### Better Alignment with Event-Driven Architecture

The platform naturally supports asynchronous processing and distributed messaging patterns.

---

## Negative Consequences

### Increased Complexity

Event sourcing introduces additional conceptual and operational complexity compared to CRUD persistence.

Areas affected include:

- aggregate modelling,
- event versioning,
- replay infrastructure,
- eventual consistency,
- projection idempotency.

---

### Eventual Consistency

Read models will lag behind the write model during asynchronous projection processing.

Consumers must tolerate temporary projection delay.

---

### Event Schema Evolution

Events are immutable once published.

Schema evolution must be carefully managed through:

- versioning,
- additive changes,
- compatibility handling.

---

### Operational Replay Costs

Projection replay may become operationally expensive for very large event streams.

Replay tooling and operational safeguards are required.

---

### More Demanding Testing

The architecture requires stronger testing discipline including:

- aggregate behaviour tests,
- replay tests,
- concurrency tests,
- projection idempotency tests.

---

# Alternatives Considered

## Traditional CRUD Persistence

### Description

Persist current entity state directly into relational tables.

### Advantages

- simpler implementation,
- familiar operational model,
- easier onboarding,
- simpler querying.

### Reasons Rejected

CRUD persistence does not naturally support:

- behavioural history,
- replayable projections,
- immutable workflow traceability,
- event-driven integration.

Additional auditing mechanisms would still be required and would likely produce fragmented historical data.

---

## CRUD with Audit Tables

### Description

Use standard relational persistence with supplementary audit history tables.

### Advantages

- lower complexity than event sourcing,
- easier reporting,
- simpler tooling.

### Reasons Rejected

Audit tables typically capture:

```text
what changed
```

but not:

```text
meaningful business behaviour
```

The approach also introduces duplication between transactional state and audit state while still lacking replay capability.

---

## Full Platform Event Sourcing

### Description

Apply event sourcing universally across all services and entities.

### Advantages

- architectural consistency,
- unified persistence model.

### Reasons Rejected

Not all domains justify event sourcing complexity.

The platform intentionally uses selective event sourcing for aggregates where:

- auditability,
- lifecycle traceability,
- and replay capability

provide meaningful value.

Simpler domains may continue using conventional persistence models.

---

# Implementation Notes

## Event Store

Azure Cosmos DB will store immutable event streams.

Recommended partition key:

```text
/streamId
```

---

## Read Models

Azure SQL Database will store query-optimised projections.

Read models are considered rebuildable derived state.

---

## Messaging

Asynchronous projections and integration events will use:

- Azure Service Bus,
- Azure Event Hubs where appropriate.

---

## Concurrency

Optimistic concurrency will be enforced at the aggregate stream level.

---

## Projection Idempotency

Projection handlers must support safe reprocessing of duplicate events.

---

# Scope

This decision applies to:

- core operational workflows,
- high-value business aggregates,
- domains requiring auditability and replayability.

This decision does not mandate event sourcing for every service or data model in the platform.

---

# Future Considerations

Potential future enhancements include:

- snapshotting,
- multi-region event replication,
- event archival strategies,
- stream compaction,
- projection replay tooling,
- event schema registry management.

These capabilities are intentionally deferred until operational complexity justifies them.

---

# References

Related documents:

- `docs/architecture/event-sourcing.md`
- `docs/architecture/overview.md`
- `docs/architecture/azure-deployment.md`