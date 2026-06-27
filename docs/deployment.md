---
layout: default
title: Deployment
permalink: /deployment/
---

# Deployment

## Overview

The Event Sourcing Reference Platform is designed for deployment as a cloud-native application on **Microsoft Azure** using fully managed Platform as a Service (PaaS) offerings.

The deployment architecture emphasises:

- Infrastructure as Code
- Automated CI/CD
- Independent service scalability
- High availability
- Observability
- Secure configuration

---

## Deployment Architecture

> **Azure Deployment Diagram**

<!--
Replace this placeholder with your exported Azure deployment diagram.

Example:

![Azure Deployment](/assets/images/azure-deployment.svg)
-->

---

## Azure Resources

| Resource | Purpose |
|----------|---------|
| Azure App Service | Hosts the ASP.NET Core Web API |
| Azure App Service | Hosts the Angular web application |
| Azure App Service WebJob | Processes projection events |
| Azure Cosmos DB | Event Store |
| Azure SQL Database | Read model projections |
| Azure Event Hubs | Event distribution |
| Azure Blob Storage | Static assets and future archival |
| Microsoft Entra ID | Authentication |
| Application Insights | Telemetry |
| Log Analytics | Centralised logging |

---

## Infrastructure as Code

All Azure resources are provisioned using **Bicep**.

Infrastructure definitions are stored alongside the application source code and deployed automatically through Azure DevOps pipelines.

Benefits include:

- Repeatable deployments
- Version-controlled infrastructure
- Environment consistency
- Automated provisioning
- Reduced configuration drift

Example deployment structure:

```text
infra/

├── main.bicep
├── appservice.bicep
├── cosmosdb.bicep
├── sql.bicep
├── eventhubs.bicep
├── storage.bicep
└── appinsights.bicep
```

---

## Continuous Integration

Every commit triggers an automated build pipeline.

Pipeline stages include:

1. Restore NuGet packages
2. Compile solution
3. Execute unit tests
4. Run static code analysis
5. Publish build artefacts

The objective is to ensure every commit produces a deployable application.

---

## Continuous Deployment

Successful builds are deployed automatically using Azure DevOps.

Typical deployment flow:

```text
Git Commit

      │

      ▼

Azure DevOps

      │

      ▼

Build

      │

      ▼

Unit Tests

      │

      ▼

Publish Artefacts

      │

      ▼

Provision Infrastructure

      │

      ▼

Deploy Application

      │

      ▼

Health Checks
```

---

## Environment Strategy

Separate Azure environments are maintained for each deployment stage.

| Environment | Purpose |
|-------------|---------|
| Development | Feature development |
| Test | Integration testing |
| Production | Live demonstration environment |

Each environment is deployed using the same Infrastructure as Code templates.

---

## Application Configuration

Configuration is externalised from the application.

Examples include:

- Connection strings
- Event Hub configuration
- Cosmos DB settings
- SQL configuration
- Feature flags
- Logging levels

Future enhancements include Azure App Configuration and Azure Key Vault integration.

---

## Security

Deployment follows Azure security best practices.

Security measures include:

- HTTPS only
- Microsoft Entra ID authentication
- Managed Identity (planned)
- Azure Key Vault (planned)
- Least privilege access
- No secrets stored in source control

---

## Observability

Production monitoring is provided through Azure Monitor services.

Monitoring includes:

- Request telemetry
- Exception tracking
- Dependency monitoring
- Performance metrics
- Availability monitoring
- Distributed tracing
- Structured logging

Application Insights provides operational visibility across all deployed components.

---

## Scaling

The architecture allows independent scaling of application components.

Examples include:

- Multiple API instances
- Independent Projection Workers
- Cosmos DB throughput scaling
- Azure SQL service tier scaling

Because processing is asynchronous, read and write workloads can be scaled independently.

---

## Resilience

The deployment architecture has been designed to support resilient operation.

Examples include:

- Stateless application services
- Retry policies
- Optimistic concurrency
- Event replay
- Independent projection processing
- Event-driven communication

Future enhancements include geo-redundancy and disaster recovery.

---

## Local Development

Developers can run the solution locally using:

- Visual Studio 2026
- .NET 10 SDK
- Azure Storage Emulator / Azurite
- Local configuration
- Azure resources where required

Infrastructure definitions remain identical between local and cloud environments wherever practical.

---

## Future Enhancements

Planned deployment improvements include:

- Blue/Green deployments
- Deployment slots
- Automated smoke testing
- OpenTelemetry
- Azure Key Vault
- Managed Identity
- GitHub Actions pipeline
- Multi-region deployment
- Disaster recovery automation

---

## Related Documentation

- [Architecture](/architecture/)
- [Technology](/technology/)
- [Event Sourcing](/event-sourcing/)
- [CQRS](/cqrs/)
- [Domain Model](/domain-model/)