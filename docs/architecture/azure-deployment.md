# Azure Deployment Architecture

## Purpose

This document describes the Azure deployment architecture used by this reference platform.

The deployment model is designed to demonstrate modern Azure-native operational practices including:

- Infrastructure as Code,
- environment isolation,
- managed identity,
- secure configuration management,
- observable cloud workloads,
- and repeatable CI/CD deployment pipelines.

The implementation prioritises architectural clarity and operational realism over cost optimisation.

---

# Deployment Principles

The platform follows several core deployment principles:

- cloud-native hosting,
- infrastructure automation,
- immutable deployments,
- least privilege security,
- environment consistency,
- observable infrastructure,
- and operational isolation.

Infrastructure should never be created manually in production environments.

All deployable resources are provisioned through Bicep templates executed by Azure DevOps pipelines.

---

# High-Level Azure Topology

The platform is deployed into Microsoft Azure using a primarily PaaS-based architecture.

High-level topology:

```text
Internet
  -> Azure Front Door (optional)
  -> App Service
  -> Backend APIs
  -> Cosmos DB Event Store
  -> Azure SQL Read Models
  -> Service Bus / Event Hubs
  -> Projection Processing
  -> Blob Storage
  -> Monitoring & Telemetry
```

The architecture intentionally minimises infrastructure management overhead by preferring managed Azure services.

---

# Azure Services

## Azure App Service

Application APIs and web applications are hosted on Azure App Service.

Responsibilities:

- API hosting,
- Angular web application hosting,
- deployment slots,
- managed TLS,
- autoscaling support.

Benefits:

- simplified operational management,
- integrated diagnostics,
- managed runtime patching,
- rapid deployment integration.

---

## Azure Cosmos DB

Cosmos DB is used as the event store.

Responsibilities:

- immutable event persistence,
- ordered aggregate event streams,
- optimistic concurrency,
- replay support.

Recommended configuration:

```text
API: Core (SQL)
Partition Key: /streamId
Consistency: Session
```

Cosmos DB is treated as the authoritative source of truth for event-sourced aggregates.

---

## Azure SQL Database

Azure SQL Database stores query-optimised read models.

Responsibilities:

- denormalised projections,
- dashboard queries,
- operational reporting,
- query APIs.

The SQL database is considered disposable and rebuildable from the event stream.

---

## Azure Service Bus

Azure Service Bus is used for asynchronous integration messaging.

Responsibilities:

- decoupled communication,
- background processing,
- workflow orchestration,
- retry handling,
- dead-lettering.

Recommended usage:

```text
Commands:
  Queue-based

Integration events:
  Topic/subscription-based
```

---

## Azure Event Hubs

Azure Event Hubs is used for high-throughput streaming scenarios.

Potential responsibilities:

- telemetry ingestion,
- operational analytics,
- downstream streaming integration,
- replayable event streaming.

Not all business events necessarily require Event Hubs.

---

## Azure Blob Storage

Blob Storage is used for:

- uploaded artefacts,
- exported reports,
- large payload storage,
- operational documents,
- replay archives.

Blob Storage should not be used as a substitute for the event store.

---

## Azure Key Vault

Key Vault centralises secret and certificate management.

Responsibilities:

- application secrets,
- certificates,
- connection strings,
- signing keys.

Applications should retrieve secrets using Managed Identity wherever possible.

---

## Azure Application Insights

Application Insights provides:

- distributed tracing,
- dependency tracking,
- request telemetry,
- exception monitoring,
- performance diagnostics.

All services should emit correlated telemetry.

---

## Azure Log Analytics

Log Analytics provides:

- centralised operational logging,
- cross-service querying,
- retention management,
- operational dashboards.

---

# Environment Strategy

The platform uses isolated deployment environments.

Example:

```text
dev
test
uat
prod
```

Each environment has:

- isolated infrastructure,
- isolated configuration,
- isolated telemetry,
- isolated databases,
- isolated messaging resources.

Cross-environment resource sharing should be avoided.

---

# Resource Group Structure

Recommended structure:

```text
rg-adsr-dev-platform
rg-adsr-test-platform
rg-adsr-prod-platform
```

Optional decomposition:

```text
rg-adsr-prod-app
rg-adsr-prod-data
rg-adsr-prod-monitoring
```

The exact structure depends on operational governance requirements.

---

# Naming Conventions

Example naming approach:

```text
adsr-dev-api-blastplanning
adsr-dev-cosmos-events
adsr-dev-sql-readmodels
adsr-dev-sb-messaging
adsr-dev-ai-monitoring
```

Naming goals:

- predictable,
- environment-aware,
- globally unique where required,
- operationally searchable.

---

# Identity and Security

## Azure Entra ID

Authentication is centralised through Azure Entra ID.

Responsibilities:

- user authentication,
- service principals,
- workload identity,
- RBAC integration.

---

## Managed Identity

Managed Identity is preferred over stored credentials.

Applications should authenticate to Azure services using:

```text
System-assigned Managed Identity
```

or:

```text
User-assigned Managed Identity
```

Avoid:

- embedded secrets,
- stored database passwords,
- long-lived access keys.

---

## RBAC

Role assignments should follow least privilege principles.

Examples:

```text
API App Service
  -> Cosmos DB Data Contributor

Projection Worker
  -> SQL Contributor

CI/CD Pipeline
  -> Resource Group Contributor
```

---

# Networking

The reference implementation intentionally begins with simplified networking for clarity.

Potential production enhancements include:

- private endpoints,
- VNet integration,
- Web Application Firewall,
- Front Door,
- API Management,
- network isolation.

These may be introduced incrementally.

---

# Infrastructure as Code

## Bicep

Infrastructure is provisioned using Bicep templates.

Goals:

- repeatability,
- version control,
- peer review,
- environment consistency,
- automated deployment.

Recommended structure:

```text
infrastructure/
  modules/
  environments/
  pipelines/
```

---

## Modules

Infrastructure should be modularised by responsibility.

Example modules:

```text
app-service.bicep
cosmosdb.bicep
sql.bicep
servicebus.bicep
monitoring.bicep
keyvault.bicep
```

---

## Environment Parameters

Environment-specific values should be externalised into parameter files.

Example:

```text
main.dev.bicepparam
main.test.bicepparam
main.prod.bicepparam
```

---

# CI/CD Architecture

## Azure DevOps

CI/CD pipelines are implemented using Azure DevOps YAML pipelines.

Pipeline responsibilities:

- build validation,
- test execution,
- IaC validation,
- what-if analysis,
- infrastructure deployment,
- application deployment,
- smoke testing.

---

## Recommended Pipeline Stages

Example deployment flow:

```text
Build
  -> Test
  -> Validate Infrastructure
  -> What-If
  -> Deploy Infrastructure
  -> Deploy Applications
  -> Smoke Tests
```

---

## Infrastructure Validation

Recommended validation steps:

```text
bicep build
az deployment group what-if
```

Infrastructure deployments should support safe preview before execution.

---

# Deployment Strategy

## Immutable Deployments

Applications should be deployed as immutable artefacts.

Avoid:

- modifying running instances manually,
- patching servers directly,
- environment drift.

---

## Deployment Slots

App Service deployment slots should be used where appropriate.

Benefits:

- warm deployments,
- reduced downtime,
- swap validation,
- rollback support.

---

# Observability Architecture

Observability is treated as a first-class architectural concern.

All services should emit:

- structured logs,
- metrics,
- traces,
- correlation identifiers.

Telemetry should support:

- incident investigation,
- distributed tracing,
- performance analysis,
- replay diagnostics.

---

# Resilience Considerations

The deployment architecture should tolerate:

- transient cloud failures,
- messaging retries,
- projection replay,
- downstream outages.

Recommended patterns:

- retry policies,
- circuit breakers,
- idempotent handlers,
- dead-letter queues,
- health checks.

---

# Scalability

The platform is designed for horizontal scaling.

Examples:

```text
App Service scale-out
Projection worker parallelism
Event Hub partition scaling
Service Bus competing consumers
```

The event store and read models may scale independently.

---

# Cost Considerations

The reference implementation prioritises architectural demonstration over minimum cost.

However, development environments should still consider:

- autoscaling limits,
- Cosmos throughput sizing,
- SQL tier selection,
- telemetry retention,
- storage lifecycle policies.

---

# Disaster Recovery

Production-oriented deployments should consider:

- geo-redundant backups,
- Cosmos DB regional replication,
- SQL point-in-time restore,
- infrastructure redeployment automation,
- projection replay capability.

Event sourcing provides strong recovery capabilities because projections can be rebuilt from immutable event history.

---

# Local Development Strategy

Local development may use:

```text
Azurite
SQL Edge
Docker Compose
Local emulators
```

The local environment should mimic production architecture where practical without introducing excessive operational overhead.

---

# Future Enhancements

Potential future enhancements include:

- Azure Front Door,
- API Management,
- Kubernetes hosting,
- private networking,
- blue/green deployments,
- chaos engineering,
- multi-region failover.

These are intentionally deferred to keep the initial implementation focused and understandable.

---

# Summary

The Azure deployment architecture is designed to demonstrate:

- modern Azure-native operational practices,
- event-driven distributed systems,
- secure cloud deployment patterns,
- observable workloads,
- and production-oriented delivery engineering.

The implementation intentionally balances realism with maintainability to provide a credible reference architecture suitable for technical discussion, experimentation, and portfolio demonstration.