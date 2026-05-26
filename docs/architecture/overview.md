# Platform Overview

## Purpose

This repository provides a reference implementation of a modern Azure-native distributed system using event sourcing, CQRS, asynchronous messaging, and cloud-native operational practices.

The project is intentionally designed to demonstrate engineering approaches commonly used in large-scale enterprise SaaS environments where:

- systems operate continuously,
- workloads are geographically distributed,
- auditability is critical,
- deployments must be repeatable and observable,
- and domain workflows evolve over time.

The implementation focuses on architectural clarity and operational realism rather than creating a feature-complete business application.

---

# High-Level Architecture

The platform follows an event-driven microservices architecture.

Core characteristics include:

- Event sourcing
- CQRS
- Asynchronous projections
- Distributed messaging
- Cloud-native infrastructure
- Immutable domain history
- Observable delivery pipelines

The system separates:

- command processing,
- event persistence,
- projection processing,
- and query workloads.

This separation allows independent scaling, resilience, and operational flexibility.

---

# Architectural Style

## Event Sourcing

The platform uses event sourcing as the system of record.

Rather than persisting only the latest entity state, the platform stores immutable domain events representing business actions.

Examples:

- BlastPlanCreated
- DrillPatternDesigned
- BlastApproved
- BlastFired

Current state is reconstructed from event streams.

Benefits include:

- complete audit history,
- temporal reconstruction,
- replay capability,
- improved traceability,
- and explicit domain behaviour modelling.

---

## CQRS

The platform separates:

- write models (commands and aggregates),
- from read models (query projections).

### Write Side

The write side validates business invariants and persists domain events.

Primary characteristics:

- transactional consistency,
- aggregate boundaries,
- domain-driven design patterns,
- immutable event persistence.

### Read Side

The read side consumes events asynchronously and builds query-optimised projections.

Primary characteristics:

- denormalised models,
- independent scaling,
- query performance optimisation,
- eventual consistency.

---

# Azure Service Topology

## Cosmos DB

Cosmos DB is used as the event store.

Responsibilities:

- immutable event persistence,
- ordered event streams,
- aggregate reconstruction,
- optimistic concurrency.

The design assumes append-only event storage.

---

## Azure SQL Database

Azure SQL Database stores read model projections.

Responsibilities:

- query optimisation,
- reporting views,
- relational querying,
- operational dashboards.

Read models are disposable and rebuildable from the event stream.

---

## Azure Service Bus

Service Bus supports asynchronous workflow orchestration and inter-service communication.

Used for:

- integration events,
- background processing,
- decoupled communication,
- retry handling.

---

## Azure Event Hubs

Event Hubs provides scalable event streaming for high-volume telemetry and event distribution scenarios.

Potential usage includes:

- operational telemetry,
- analytics pipelines,
- streaming integration.

---

## Azure App Service

Application services are hosted using Azure App Service.

Benefits:

- simplified operational hosting,
- managed scaling,
- deployment slot support,
- integrated monitoring.

---

## Azure Blob Storage

Blob Storage is used for large object persistence.

Examples:

- uploaded files,
- exported reports,
- operational artefacts.

---

# Domain Structure

The initial implementation models a simplified industrial blast planning workflow.

The purpose is to provide a realistic domain containing:

- long-running workflows,
- operational state transitions,
- auditable events,
- asynchronous processing requirements.

The domain is intentionally representative rather than production-specific.

---

# Bounded Contexts

Initial bounded contexts may include:

## Blast Planning

Responsible for:

- blast plan lifecycle,
- drill pattern definition,
- approval workflows.

## Loading Operations

Responsible for:

- explosive loading activities,
- loading status tracking,
- operational validation.

## Reconciliation

Responsible for:

- post-blast reconciliation,
- operational metrics,
- historical reporting.

---

# Deployment Philosophy

Infrastructure is provisioned entirely using Infrastructure as Code.

Primary goals:

- repeatability,
- environment consistency,
- automated validation,
- traceable infrastructure changes.

The repository uses:

- Bicep,
- ARM templates,
- Azure DevOps pipelines.

---

# Observability Strategy

The platform is designed to be observable from inception.

Observability includes:

- structured logging,
- distributed tracing,
- telemetry correlation,
- metrics collection,
- centralised diagnostics.

Azure services used:

- Application Insights
- Log Analytics

Operational visibility is treated as a core architectural concern rather than a post-deployment enhancement.

---

# Security Model

The platform follows a cloud-native security model.

Core principles:

- least privilege access,
- managed identity usage,
- centralised identity management,
- secret externalisation,
- RBAC enforcement.

Authentication is implemented using Azure Entra ID.

---

# Engineering Goals

This repository aims to demonstrate:

- architectural decision-making,
- distributed systems design,
- operational engineering practices,
- cloud-native delivery,
- and maintainable enterprise application structure.

The implementation prioritises:

- explicit trade-offs,
- simplicity where possible,
- operational transparency,
- and evolutionary architecture.

---

# Non-Goals

The repository is not intended to:

- replicate a production mining platform,
- provide a complete business solution,
- maximise feature count,
- or demonstrate every Azure service.

The focus is architectural quality rather than breadth.

---

# Intended Audience

This repository is primarily intended for:

- software architects,
- senior engineers,
- technical leads,
- and engineering managers

interested in modern Azure-native distributed system design patterns.