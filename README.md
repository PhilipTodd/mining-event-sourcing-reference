# Azure Distributed Systems Reference

A reference implementation of a modern Azure-native distributed system using event sourcing, CQRS, cloud-native messaging, and observable delivery pipelines.

This project is intended to demonstrate production-oriented architectural patterns commonly used in large-scale enterprise SaaS platforms operating in globally distributed, high-availability environments.

---

# Objectives

This repository demonstrates:

- Event-sourced architecture
- CQRS with asynchronous projections
- Distributed microservice boundaries
- Azure-native integration patterns
- Cloud-first operational design
- Infrastructure as Code (IaC)
- CI/CD automation
- Observability and telemetry
- Production-oriented engineering practices

The implementation intentionally prioritises architectural clarity, operational realism, and maintainability over unnecessary complexity.

---

# Technology Stack

## Backend

- .NET 10
- C#
- ASP.NET Core Web API
- Minimal APIs
- MediatR
- FluentValidation

## Frontend

- Angular

## Cloud Platform

- Microsoft Azure

## Azure Services

- Azure App Service
- Azure Cosmos DB
- Azure SQL Database
- Azure Service Bus
- Azure Event Hubs
- Azure Blob Storage
- Azure Key Vault
- Azure Application Insights
- Azure Log Analytics

## Identity & Security

- Azure Entra ID
- Managed Identity
- RBAC

## Infrastructure as Code

- Bicep
- ARM Templates

## DevOps

- Azure DevOps Pipelines
- YAML pipelines
- GitHub

---

# Architecture Overview

The platform follows an event-driven architecture using event sourcing and CQRS.

## Write Side

Commands are processed by aggregates which persist immutable domain events to Cosmos DB.

Example flow:

```text
Client
  -> API
  -> Command Handler
  -> Aggregate
  -> Event Store (Cosmos DB)
```

## Read Side

Domain events are projected asynchronously into query-optimised read models stored in Azure SQL Database.

```
Event Store
  -> Projection Processor
  -> Azure SQL Read Models
  -> Query API
```

## Messaging

Asynchronous integration uses:

- Azure Service Bus
- Azure Event Hubs
 
## Initial Domain

The initial bounded context models a simplified blast planning workflow inspired by industrial mining operations.

Example domain events:

- BlastPlanCreated
- DrillPatternDesigned
- HoleLoaded
- BlastApproved
- BlastFired
- BlastReconciled

## Repository Structure

```
src/
  backend/
    services/
    building-blocks/
  frontend/
  infrastructure/
  tests/

docs/
  architecture/
  adr/
  diagrams/

.azuredevops/

.github/
```

## Architectural Principles

- Explicit bounded contexts
- Immutable event history
- Asynchronous integration
- Autonomous deployability
- Cloud-native observability
- Infrastructure automation
- Operational transparency
- Secure-by-default design


## Observability

The platform includes:

- Structured logging
- Distributed tracing
- Metrics
- Centralised log aggregation
- Application Insights telemetry
- Correlation IDs across workflows


## CI/CD

Deployment pipelines are implemented using Azure DevOps YAML pipelines.

Pipeline goals:

- Automated validation
- Infrastructure deployment
- Environment promotion
- Secure secret handling
- Repeatable deployments
- Deployment observability

## Infrastructure

Infrastructure is provisioned entirely through Bicep templates.

Environment targets:

- Local development
- Development
- Test
- Production


## Local Development

### Prerequisites
- .NET 10 SDK
- Node.js LTS
- Angular CLI
- Docker Desktop
- Azure CLI
- Visual Studio 2026 or VS Code

### Build

```
dotnet build
```
### Run API

```
dotnet run --project src/backend/services/blast-planning/BlastPlanning.Api
```

## Roadmap

### Phase 1

- Initial bounded context
- Event store abstraction
- Cosmos DB persistence
- SQL projections
- Basic Angular UI

### Phase 2

- Messaging integration
- Distributed workflows
- Authentication and authorisation
- Telemetry and tracing

### Phase 3

- Full IaC deployment
- CI/CD pipelines
- Load and resilience testing
- Chaos engineering experiments

## Status

Active development.

This repository is intentionally being developed incrementally to demonstrate architecture evolution, delivery practices, and engineering trade-offs over time.

## License

MIT License
