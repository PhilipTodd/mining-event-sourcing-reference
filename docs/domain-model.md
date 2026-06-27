---
layout: default
title: Domain Model
permalink: /domain-model/
---

# Domain Model

## Overview

The Event Sourcing Reference Platform applies **Domain-Driven Design (DDD)** to model the core business concepts involved in planning and executing mining blasts.

Business rules are encapsulated within aggregates rather than being distributed throughout controllers, services or persistence layers.

This approach produces a domain model that is expressive, testable and independent of infrastructure concerns.

---

## Domain-Driven Design

The solution follows several core DDD principles.

- Rich domain model
- Aggregates enforce business invariants
- Immutable domain events
- Value Objects for strongly typed concepts
- Commands express business intent
- Ubiquitous language throughout the codebase

---

## Bounded Context

The current implementation focuses on a single bounded context.

```text
Blast Planning
```

Future iterations may introduce additional bounded contexts, including:

- Explosives Management
- Inventory
- Drill Operations
- Blast Execution
- Reporting
- Notifications

Each bounded context would own its own domain model and persistence.

---

# Aggregate

The primary aggregate is **BlastPlan**.

The aggregate is responsible for enforcing all business rules associated with planning a blast.

```text
BlastPlan
│
├── Identity
├── Status
├── Blast Pattern
├── Charge Design
├── Approval Information
└── Event History
```

The aggregate represents the transactional consistency boundary.

No external component modifies aggregate state directly.

---

## Aggregate Responsibilities

The BlastPlan aggregate is responsible for:

- Creating new blast plans
- Managing lifecycle state
- Validating business rules
- Producing domain events
- Preventing invalid state transitions

---

## Aggregate Lifecycle

```text
Draft

   │

   ▼

Designed

   │

   ▼

Approved

   │

   ▼

Executed

   │

   ▼

Completed
```

Business rules determine which transitions are permitted.

---

# Commands

Commands express business intent.

Examples include:

- CreateBlastPlan
- UpdateBlastPattern
- CompleteChargeDesign
- ApproveBlastPlan
- ExecuteBlast

Commands are validated before reaching the aggregate.

Successful commands produce one or more domain events.

---

# Domain Events

Domain events represent facts that have already occurred.

Examples include:

- BlastPlanCreated
- BlastPatternUpdated
- ChargeDesignCompleted
- BlastApproved
- BlastExecuted

Events are immutable and become part of the permanent business history.

---

## Event Stream

Each aggregate maintains an ordered event stream.

```text
BlastPlanCreated

        │

BlastPatternUpdated

        │

ChargeDesignCompleted

        │

BlastApproved

        │

BlastExecuted
```

The current aggregate state is reconstructed by replaying these events.

---

# Value Objects

The domain model uses Value Objects to represent concepts that have identity through their attributes rather than an identifier.

Examples include:

- BlastLocation
- HoleSpacing
- Burden
- ExplosiveCharge
- DelayTiming

Value Objects are immutable and validated upon creation.

---

# Entities

Entities possess a unique identity that persists over time.

Current entities include:

- BlastPlan

Future entities may include:

- BlastHole
- DrillPattern
- ExplosiveProduct
- Detonator
- ChargePlan

---

# Business Rules

Examples of business rules enforced by the aggregate include:

- A Blast Plan cannot be approved before the charge design is complete.
- An executed blast cannot be modified.
- A completed blast cannot be deleted.
- Required information must exist before approval.
- Aggregate state must remain internally consistent.

These rules are enforced within the domain model rather than the user interface.

---

# Repository

Repositories provide persistence abstraction for aggregates.

Responsibilities include:

- Loading event streams
- Saving newly generated events
- Applying optimistic concurrency
- Hiding infrastructure implementation details

The domain model remains independent of Cosmos DB and Azure services.

---

# Domain Services

Most business logic resides within aggregates.

Domain Services are introduced only where behaviour spans multiple aggregates or cannot naturally belong to a single entity.

Current implementation:

- None

Planned examples:

- Blast Validation Service
- Scheduling Service
- Risk Assessment Service

---

# Domain Events vs Integration Events

This project distinguishes between internal domain events and external integration events.

## Domain Events

- Internal
- Business focused
- Aggregate lifecycle
- Persisted in the Event Store

Examples:

- BlastApproved
- BlastExecuted

## Integration Events

- External
- Used for communication between services
- Published after successful event persistence

Examples:

- BlastApprovedIntegrationEvent
- BlastExecutedIntegrationEvent

This separation allows the internal domain model to evolve independently from external consumers.

---

# Domain Model Summary

```text
Commands

      │

      ▼

Aggregate

      │

Business Rules

      │

      ▼

Domain Events

      │

      ▼

Cosmos DB Event Store

      │

      ▼

Projection Workers

      │

      ▼

Azure SQL Read Models
```

---

# Future Enhancements

The domain model will continue to evolve as additional functionality is implemented.

Planned enhancements include:

- Multiple aggregates
- Additional bounded contexts
- Domain services
- Specification pattern
- Snapshot support
- Event versioning
- Aggregate factories
- Cross-context integration

---

## Related Documentation

- [Architecture](/architecture/)
- [Technology](/technology/)
- [Event Sourcing](/event-sourcing/)
- [CQRS](/cqrs/)
- [Deployment](/deployment/)