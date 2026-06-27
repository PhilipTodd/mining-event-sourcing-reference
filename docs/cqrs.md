---
layout: default
title: CQRS
permalink: /cqrs/
---

# Command Query Responsibility Segregation (CQRS)

## Overview

The Event Sourcing Reference Platform implements **Command Query Responsibility Segregation (CQRS)** to separate the responsibilities of modifying application state from querying application data.

Rather than using a single model for both reads and writes, CQRS enables each side of the application to be independently designed, optimised and scaled.

This approach complements Event Sourcing by allowing immutable event streams to drive specialised read models tailored to the needs of the user interface.

---

## Why CQRS?

Traditional CRUD applications typically use a single data model for both updating and querying data.

As systems become larger and more complex, this approach often results in:

- Complex domain models
- Inefficient database queries
- Tight coupling between business logic and reporting
- Difficult scaling
- Complicated persistence models

CQRS addresses these issues by separating write operations from read operations.

---

## High-Level Flow

```text
                    User

                     ?
        ???????????????????????????
        ?                         ?
        ?                         ?

   Commands                  Queries

        ?                         ?

        ?                         ?

Command Handlers           Query Handlers

        ?                         ?

        ?                         ?

 Domain Aggregate         Azure SQL Read Model

        ?                         ?

        ?                         ?

   Domain Events          Read DTOs

        ?

        ?

 Azure Cosmos DB
    Event Store
```

---

## Commands

Commands represent requests to change business state.

A command expresses **intent**, not implementation.

Examples include:

- Create Blast Plan
- Update Blast Pattern
- Approve Blast
- Execute Blast

Commands are validated before reaching the domain model.

A successful command produces one or more domain events.

Commands do **not** return business data.

Typical response:

- Success
- Failure
- Validation errors

---

## Command Processing

```text
HTTP Request

      ?

      ?

Command DTO

      ?

      ?

Command Handler

      ?

      ?

Load Aggregate

      ?

      ?

Business Rules

      ?

      ?

Domain Events

      ?

      ?

Persist Events
```

The aggregate is responsible for enforcing business invariants.

---

## Queries

Queries retrieve information without modifying application state.

Unlike commands, queries are optimised for presentation rather than business behaviour.

Queries access Azure SQL read models rather than the Event Store.

This allows:

- Fast response times
- Complex filtering
- Reporting
- Dashboard support
- Pagination
- Aggregation

---

## Query Processing

```text
HTTP Request

      ?

      ?

Query DTO

      ?

      ?

Query Handler

      ?

      ?

Azure SQL

      ?

      ?

Response DTO
```

Queries never execute business logic or create domain events.

---

## Read Models

Read models are denormalised views of business data created specifically for querying.

Rather than reconstructing aggregates from events for every request, projection workers continuously update read models.

Benefits include:

- Fast queries
- Simple SQL
- Optimised indexing
- Independent schema evolution

---

## Eventual Consistency

CQRS introduces eventual consistency between the write and read models.

The sequence is:

```text
Command

    ?

    ?

Event Store

    ?

    ?

Event Hubs

    ?

    ?

Projection Worker

    ?

    ?

Azure SQL

    ?

    ?

Query
```

Immediately after a command completes, the read model may briefly lag behind until projection processing finishes.

For most business applications this delay is measured in milliseconds or seconds.

---

## Benefits

CQRS provides several architectural advantages.

### Independent Optimisation

Write models prioritise business correctness.

Read models prioritise query performance.

---

### Independent Scaling

Command processing and query processing can scale independently.

For example:

- Increase API instances for command-heavy workloads.
- Scale projection workers independently.
- Scale Azure SQL separately from Cosmos DB.

---

### Simplified Domain Model

The write model focuses exclusively on business rules.

Presentation concerns remain isolated within read models.

---

### Flexible Read Models

Different user interfaces often require different data.

CQRS allows multiple read models to be generated from the same event stream.

Examples include:

- Operational dashboard
- Reporting
- Mobile application
- Analytics
- External integrations

---

## Trade-offs

CQRS introduces additional complexity compared to CRUD systems.

### Advantages

- Clear separation of responsibilities
- High-performance queries
- Independent scalability
- Cleaner domain model
- Excellent fit for Event Sourcing

### Challenges

- Additional infrastructure
- Eventual consistency
- Projection maintenance
- More application components
- Increased operational complexity

For enterprise systems with complex business rules, these trade-offs are frequently justified.

---

## CQRS in this Project

This reference implementation includes:

### Command Side

- ASP.NET Core Web API
- Command Handlers
- Domain Aggregates
- Azure Cosmos DB Event Store

### Read Side

- Projection Workers
- Azure SQL
- Query Handlers
- Read DTOs

The write model and read model are intentionally isolated to demonstrate a production-oriented CQRS architecture.

---

## Future Enhancements

Future iterations will include:

- Multiple read models
- Projection replay
- Read model versioning
- Snapshot support
- Read model caching
- Distributed projections

---

## Related Documentation

- [Architecture](/architecture/)
- [Technology](/technology/)
- [Event Sourcing](/event-sourcing/)
- [Deployment](/deployment/)