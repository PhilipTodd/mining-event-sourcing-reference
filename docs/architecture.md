---
layout: default
title: Architecture
permalink: /architecture/
---

# Architecture

## Overview

The **Event Sourcing Reference Platform** demonstrates a production-style cloud-native architecture built on Microsoft Azure using **Event Sourcing**, **CQRS**, and **Domain-Driven Design (DDD)**.

Rather than serving as a proof of concept, this project has been designed to demonstrate architectural patterns commonly used in enterprise systems that require auditability, scalability, resiliency, and eventual consistency.

---

## Architecture Principles

The solution has been designed around the following principles:

- Event-first persistence
- Clear separation of write and read models
- Rich domain model encapsulating business rules
- Asynchronous event processing
- Independent scalability of application components
- Cloud-native Azure PaaS services
- Infrastructure as Code
- Observability by design

---

## High-Level Architecture

> **Diagram coming soon**

<!--
Replace this placeholder with your exported Structurizr diagram.

Example:

![C4 Container Diagram](/assets/images/c4-container.svg)
-->

---

## Request Lifecycle

A typical command follows the sequence below.

1. User submits a command from the Angular application.
2. ASP.NET Core validates the request.
3. Command Handler loads the Aggregate.
4. Aggregate executes business rules.
5. One or more Domain Events are produced.
6. Events are persisted to Azure Cosmos DB.
7. Events are published to Azure Event Hubs.
8. Projection Workers process new events.
9. Azure SQL read models are updated.
10. Queries retrieve optimized read models.

---

## Architectural Patterns

| Pattern | Purpose |
|----------|---------|
| Event Sourcing | Store immutable domain events rather than current state |
| CQRS | Separate command and query responsibilities |
| Domain-Driven Design | Encapsulate business logic inside aggregates |
| Event-Driven Architecture | Decouple components using asynchronous messaging |
| Repository Pattern | Abstract persistence concerns |
| Dependency Injection | Improve modularity and testability |
| Eventual Consistency | Optimise read performance independently from writes |

---

## Solution Architecture

```text
                Angular Web Application
                         │
                         ▼
               ASP.NET Core Web API
                         │
                Command Handlers
                         │
                         ▼
                  Domain Aggregate
                         │
                  Domain Events
                         │
                         ▼
               Azure Cosmos DB
                  Event Store
                         │
                         ▼
                Azure Event Hubs
                         │
              Projection Workers
                         │
                         ▼
               Azure SQL Database
                  Read Models
                         │
                         ▼
                  Query Handlers
                         │
                         ▼
                 Angular Web UI
```

---

## Azure Services

| Service | Responsibility |
|----------|----------------|
| Azure App Service | Hosts API and web application |
| Azure Cosmos DB | Event Store |
| Azure SQL Database | Read model projections |
| Azure Event Hubs | Event distribution |
| Azure Blob Storage | Static assets and future event archival |
| Microsoft Entra ID | Authentication and authorization |
| Application Insights | Telemetry |
| Log Analytics | Centralised logging |
| Azure DevOps | Continuous Integration and Deployment |
| Bicep | Infrastructure as Code |

---

## Design Decisions

### Event Sourcing

Business state is derived from an immutable sequence of events rather than storing only the current state. This provides a complete audit history and supports event replay.

### CQRS

Commands and queries are implemented independently. This allows transactional processing and reporting workloads to evolve separately.

### Cosmos DB Event Store

Azure Cosmos DB provides scalable storage for immutable event streams while supporting optimistic concurrency for aggregate consistency.

### SQL Read Models

Read models are projected into Azure SQL to support efficient querying, reporting and dashboard scenarios.

### Event Hubs

Azure Event Hubs distributes committed events to downstream consumers without tightly coupling projection processing to command execution.

---

## Project Structure

```text
src/
├── Api/
├── Application/
├── Domain/
├── Infrastructure/
└── ProjectionWorker/

infra/
├── bicep/

tests/
├── UnitTests/
└── IntegrationTests/
```

---

## Scalability

The architecture allows each component to scale independently.

- Multiple API instances
- Independent Projection Workers
- Partitioned Event Store
- Independent read database scaling
- Asynchronous event processing
- Stateless application services

---

## Observability

Operational visibility is provided through:

- Structured logging
- Distributed tracing
- Application Insights
- Azure Monitor
- Log Analytics
- Health endpoints
- Metrics collection

---

## Security

Security considerations include:

- Microsoft Entra ID authentication
- Role-based authorization
- HTTPS throughout
- Managed Identities
- Secure configuration using Azure App Configuration and Key Vault (future enhancement)
- Principle of least privilege

---

## Future Enhancements

Planned enhancements include:

- Snapshotting
- Outbox Pattern
- Inbox Pattern
- Saga orchestration
- Event versioning
- Multi-region deployment
- Geo-replication
- Distributed tracing with OpenTelemetry
- Blue/Green deployments

---

## Related Documentation

- [Technology Stack](/technology/)
- [Event Sourcing](/event-sourcing/)
- [CQRS](/cqrs/)
- [Deployment](/deployment/)
- [Roadmap](/roadmap/)