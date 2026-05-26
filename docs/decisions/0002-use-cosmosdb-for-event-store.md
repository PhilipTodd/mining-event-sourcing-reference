# ADR 0002: Use Azure Cosmos DB as the Event Store

## Status

Accepted

---

# Context

The platform uses event sourcing for selected operational workflows.

This requires a persistence mechanism capable of supporting:

- immutable append-only event storage,
- ordered aggregate event streams,
- optimistic concurrency,
- horizontal scalability,
- low-latency writes,
- replayable historical data,
- and cloud-native operational characteristics.

The event store is a critical architectural component because it becomes the authoritative system of record for event-sourced aggregates.

The platform is intended to demonstrate modern Azure-native distributed systems practices using primarily managed Azure PaaS services.

---

# Decision

Azure Cosmos DB will be used as the primary event store for event-sourced aggregates.

The implementation will use:

```text
Azure Cosmos DB Core (SQL) API
```

Event streams will be stored as immutable event documents.

Recommended partition key:

```text
/streamId
```

Each aggregate instance will own an ordered event stream.

Example:

```text
blast-plan-4f3c2a
```

The event store will support:

- aggregate reconstruction,
- optimistic concurrency,
- replay operations,
- asynchronous projections,
- integration event publication.

Azure SQL Database will not be used as the primary event store.

Instead, Azure SQL Database will be used for read model projections.

---

# Rationale

Azure Cosmos DB was selected because it aligns strongly with the operational and architectural goals of the platform.

---

## Cloud-Native Scalability

Cosmos DB provides horizontally scalable storage with managed partitioning and throughput scaling.

This aligns well with:

- distributed workloads,
- event-driven processing,
- independently scalable services.

---

## Low-Latency Writes

Event sourcing prioritises reliable append operations.

Cosmos DB provides predictable low-latency writes suitable for append-heavy workloads.

---

## Flexible Event Schema Storage

Events naturally evolve over time.

Cosmos DB supports schema-flexible JSON document storage without requiring rigid relational schema migrations for every event version.

Example:

```json
{
  "eventType": "BlastPlanApproved",
  "eventVersion": 2,
  "data": {
    "approvedBy": "user-123",
    "approvedUtc": "2026-05-26T01:15:00Z",
    "approvalReason": "Operational review completed"
  }
}
```

This simplifies event evolution.

---

## Aggregate-Oriented Access Patterns

Event sourcing typically loads events by aggregate stream.

Recommended partitioning:

```text
/streamId
```

supports efficient:

- stream reads,
- ordered replay,
- aggregate reconstruction.

---

## Managed PaaS Operations

Cosmos DB reduces infrastructure management overhead.

Benefits include:

- automatic replication,
- managed backups,
- automatic indexing,
- elastic scaling,
- multi-region support,
- SLA-backed availability.

This aligns with the project’s Azure-native deployment goals.

---

## Operational Alignment

The project intentionally mirrors architectural patterns commonly associated with modern Azure-native enterprise systems.

Using Cosmos DB for the event store demonstrates familiarity with:

- cloud-native persistence,
- distributed consistency trade-offs,
- partition-oriented design,
- operational scalability patterns.

---

# Consequences

## Positive Consequences

### Horizontally Scalable Event Storage

The event store can scale independently from query infrastructure.

---

### Flexible Event Evolution

Schema-flexible JSON storage simplifies event contract evolution.

---

### Strong Alignment with Event Sourcing Access Patterns

Partitioning by stream supports efficient aggregate reconstruction.

---

### Simplified Operations

Cosmos DB removes the need to manage:

- database clusters,
- storage replication,
- failover infrastructure,
- patching operations.

---

### Multi-Region Support

Cosmos DB supports geographically distributed deployments if required in future iterations.

---

## Negative Consequences

### Higher Cost Model

Cosmos DB can become expensive if:

- partitioning is poor,
- throughput is oversized,
- queries are inefficient,
- event payloads grow excessively.

RU consumption must be monitored carefully.

---

### Partition Design Complexity

Incorrect partitioning can create:

- hot partitions,
- uneven throughput distribution,
- replay bottlenecks.

Partition strategy becomes a critical architectural concern.

---

### Event Ordering Limitations

Cosmos DB guarantees ordering only within a logical partition.

Global event ordering across all aggregates is not guaranteed.

The architecture must avoid relying on global ordering semantics.

---

### Querying Limitations

Cosmos DB is not intended for complex relational reporting workloads.

This is intentional.

The platform separates:

- transactional event persistence,
- from relational query models.

---

### Operational Replay Considerations

Large-scale replay operations may generate significant RU consumption.

Replay tooling and operational throttling may be required.

---

# Alternatives Considered

## Azure SQL Database as Event Store

### Description

Store events in relational append-only tables.

### Advantages

- familiar tooling,
- strong transactional semantics,
- simpler ad-hoc querying,
- relational constraints.

### Reasons Rejected

Relational databases are less naturally aligned with:

- partition-oriented scalability,
- document-based event payloads,
- cloud-native distributed event workloads.

Using Azure SQL for projections and Cosmos DB for event persistence provides clearer workload separation.

---

## EventStoreDB

### Description

Use a specialised event sourcing database platform.

### Advantages

- purpose-built event sourcing semantics,
- native stream support,
- projection tooling,
- subscription mechanisms.

### Reasons Rejected

The project intentionally prioritises:

- Azure-native managed services,
- reduced operational overhead,
- alignment with Azure enterprise ecosystems.

Introducing EventStoreDB would increase operational complexity and infrastructure management requirements.

---

## PostgreSQL Event Store

### Description

Store append-only event streams in PostgreSQL tables.

### Advantages

- lower operational cost,
- strong SQL ecosystem,
- mature tooling.

### Reasons Rejected

The project is intentionally Azure-centric and focused on demonstrating Azure-native distributed systems architecture patterns.

---

## Blob Storage Event Archive

### Description

Persist events as files or blobs.

### Advantages

- low storage cost,
- simple archival model.

### Reasons Rejected

Blob storage does not provide suitable semantics for:

- aggregate reconstruction,
- optimistic concurrency,
- efficient stream access,
- transactional append workflows.

---

# Implementation Notes

## Container Design

Recommended Cosmos container structure:

```text
Container:
  events
```

Partition key:

```text
/streamId
```

---

## Event Document Structure

Example:

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
  "data": {
    "blastPlanId": "4f3c2a",
    "approvedBy": "user-123"
  }
}
```

---

## Consistency Model

Recommended consistency level:

```text
Session
```

This provides:

- read-your-own-write semantics,
- lower latency than strong consistency,
- predictable aggregate behaviour.

---

## Concurrency

Optimistic concurrency should be enforced using:

- aggregate sequence versioning,
- ETag validation where appropriate.

---

## Replay

Replay tooling must support:

- controlled throughput,
- projection rebuild,
- replay checkpointing,
- replay observability.

---

## Retention

Events are immutable and should generally be retained indefinitely unless:

- regulatory requirements,
- archival policies,
- or storage governance

require otherwise.

---

# Scope

This decision applies to:

- event-sourced aggregates,
- immutable domain event persistence,
- replayable operational workflows.

This decision does not require Cosmos DB usage for:

- reporting workloads,
- analytical workloads,
- relational querying,
- projection storage.

---

# Future Considerations

Potential future enhancements include:

- multi-region replication,
- event archival strategies,
- cold storage migration,
- snapshot support,
- replay optimisation,
- dedicated projection feeds,
- change feed processors.

These capabilities should be introduced only when justified by operational scale.

---

# References

Related documents:

- `docs/architecture/event-sourcing.md`
- `docs/architecture/overview.md`
- `docs/architecture/azure-deployment.md`
- `docs/adr/0001-use-event-sourcing.md`