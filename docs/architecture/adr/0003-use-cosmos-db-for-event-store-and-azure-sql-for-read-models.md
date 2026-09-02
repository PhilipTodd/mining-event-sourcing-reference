---
layout: page
title: Use Cosmos DB for the Event Store and Azure SQL for Read Models
eyebrow: ADR 0003
permalink: /architecture/adr/0003-use-cosmos-db-for-event-store-and-azure-sql-for-read-models/
---

## Context and Problem Statement

The Event Sourcing Reference application requires two distinct persistence models: an append-oriented store for immutable domain events and a query-optimised store for application read models. Using a single database technology for both concerns would simplify infrastructure but would not align as well with the different access patterns of event storage and projection querying.

The persistence architecture should demonstrate a realistic CQRS and event-sourcing approach while remaining suitable for Azure PaaS hosting.

## Considered Options

* Use Azure Cosmos DB for the event store and Azure SQL for read models
* Use Azure SQL for both event storage and read models
* Use Azure Cosmos DB for both event storage and read models
* Use a dedicated event-store product with a separate relational read model

## Decision Outcome

Chosen option: **"Use Azure Cosmos DB for the event store and Azure SQL for read models"**, because the two technologies align well with the different persistence requirements of the write and read sides of the application.

Azure Cosmos DB stores the immutable event streams for blast plan aggregates. Its document-oriented model is well suited to storing serialized domain events and allows event streams to be retrieved by aggregate identity.

Azure SQL stores denormalised read models such as `BlastPlanning.BlastPlanSummary`, providing efficient relational querying for application views and API query endpoints.

This separation reinforces the CQRS model: the event store is the source of truth for aggregate state, while Azure SQL contains projections derived from those events.

### Consequences

* Good, because the persistence technology for each side of the system is selected according to its access pattern rather than forcing both concerns into a single data model.
* Good, because Cosmos DB provides a natural document-oriented representation for immutable serialized domain events.
* Good, because Azure SQL provides mature relational querying, indexing, filtering, and reporting capabilities for read models.
* Good, because read models can be rebuilt from the event store if projections need to be recreated.
* Good, because the architecture clearly demonstrates CQRS by separating the authoritative write model from query-optimised projections.
* Good, because the two persistence layers can be scaled and tuned independently.
* Bad, because operating two database technologies increases infrastructure and application complexity.
* Bad, because the read model is eventually consistent with the event store rather than being updated transactionally with command processing.
* Bad, because projection failures or processing delays can cause the SQL read model to temporarily lag behind the authoritative event stream.
* Bad, because consistency and ordering must be handled explicitly when asynchronously projecting events into SQL.
* Bad, because Cosmos DB partition-key design and request-unit consumption must be considered as event volume increases.