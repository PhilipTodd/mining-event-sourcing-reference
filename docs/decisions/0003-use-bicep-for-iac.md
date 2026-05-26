# ADR 0003: Use Bicep for Infrastructure as Code

## Status

Accepted

---

# Context

The platform requires repeatable, version-controlled provisioning of Azure infrastructure across multiple environments.

The deployment architecture includes Azure resources such as:

- App Service,
- Cosmos DB,
- Azure SQL Database,
- Service Bus,
- Event Hubs,
- Key Vault,
- Application Insights,
- Blob Storage,
- and supporting monitoring resources.

The project aims to demonstrate modern cloud-native engineering practices including:

- automated infrastructure provisioning,
- environment consistency,
- CI/CD integration,
- operational repeatability,
- and infrastructure governance.

Manual infrastructure provisioning through the Azure Portal would introduce:

- environment drift,
- undocumented changes,
- inconsistent configuration,
- reduced auditability,
- and operational risk.

The project is intentionally Azure-native and designed to align with enterprise Azure delivery practices.

---

# Decision

The platform will use Bicep as the primary Infrastructure as Code (IaC) technology.

All Azure infrastructure will be provisioned using:

```text
Bicep templates
```

executed through:

```text
Azure DevOps YAML pipelines
```

Infrastructure definitions will be stored in source control alongside application code.

Recommended repository structure:

```text
infrastructure/
  modules/
  environments/
  pipelines/
```

Environment-specific configuration will be externalised into parameter files.

Example:

```text
main.dev.bicepparam
main.test.bicepparam
main.prod.bicepparam
```

The platform will not rely on manual Azure Portal provisioning for managed environments.

---

# Rationale

Bicep was selected because it aligns strongly with the project’s Azure-native architecture and operational goals.

---

## Azure-Native IaC

Bicep is Microsoft’s recommended Infrastructure as Code language for Azure resource deployment.

It provides:

- first-class Azure support,
- native ARM integration,
- strong tooling support,
- simplified Azure resource modelling.

This aligns directly with the platform’s Azure-centric architecture.

---

## Improved Readability

Bicep provides significantly improved readability compared to raw ARM JSON templates.

Example:

Bicep:

```bicep
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'B1'
  }
}
```

Equivalent ARM JSON is substantially more verbose and difficult to maintain.

Improved readability supports:

- maintainability,
- peer review,
- onboarding,
- architectural clarity.

---

## Infrastructure Version Control

Storing infrastructure definitions in Git enables:

- change history,
- peer review,
- rollback capability,
- deployment traceability,
- operational governance.

Infrastructure becomes part of the engineering delivery lifecycle rather than an external operational concern.

---

## Environment Consistency

Bicep supports consistent provisioning across:

```text
dev
test
uat
prod
```

This reduces:

- configuration drift,
- deployment inconsistency,
- undocumented manual changes.

---

## CI/CD Integration

Bicep integrates cleanly with:

- Azure CLI,
- Azure DevOps,
- GitHub Actions,
- ARM deployment workflows.

This simplifies deployment automation and validation.

---

## Modular Infrastructure Design

Bicep supports reusable modules.

Example:

```text
modules/
  app-service.bicep
  cosmosdb.bicep
  sql.bicep
  servicebus.bicep
  monitoring.bicep
```

This improves:

- reuse,
- consistency,
- composability,
- maintainability.

---

## What-If Deployment Validation

Azure deployments support:

```text
az deployment group what-if
```

This allows previewing infrastructure changes before execution.

Benefits include:

- safer deployments,
- infrastructure diff visibility,
- reduced operational risk.

---

## Alignment with Azure Enterprise Practices

Bicep is increasingly standard across Azure enterprise delivery environments.

Using Bicep demonstrates familiarity with:

- Azure-native DevOps workflows,
- infrastructure automation,
- operational governance,
- cloud engineering practices.

---

# Consequences

## Positive Consequences

### Repeatable Infrastructure

Environments can be recreated consistently and predictably.

---

### Reduced Manual Configuration

Operational dependence on Azure Portal configuration is minimised.

---

### Improved Governance

Infrastructure changes become reviewable and auditable through Git workflows.

---

### Better Disaster Recovery

Infrastructure can be recreated from source-controlled templates.

---

### Strong CI/CD Integration

Infrastructure deployments become part of automated delivery pipelines.

---

### Improved Maintainability

Reusable modules simplify long-term infrastructure evolution.

---

## Negative Consequences

### Learning Curve

Bicep introduces additional concepts including:

- modular deployment design,
- parameter management,
- deployment scopes,
- dependency modelling.

---

### Azure-Specific Coupling

Bicep is Azure-specific.

The platform becomes more tightly coupled to Azure deployment tooling.

This is considered acceptable because the project is intentionally Azure-native.

---

### IaC Maintenance Overhead

Infrastructure definitions require ongoing maintenance alongside application evolution.

Examples:

- API version updates,
- resource schema changes,
- module refactoring.

---

### Deployment Complexity

Complex environments may require:

- staged deployments,
- dependency orchestration,
- RBAC coordination,
- environment-specific overrides.

---

# Alternatives Considered

## Manual Azure Portal Provisioning

### Description

Provision infrastructure manually through the Azure Portal.

### Advantages

- simple initial setup,
- minimal IaC knowledge required.

### Reasons Rejected

Manual provisioning introduces:

- configuration drift,
- undocumented changes,
- inconsistent environments,
- operational fragility,
- reduced reproducibility.

This approach does not align with modern cloud engineering practices.

---

## ARM Templates

### Description

Use raw ARM JSON templates directly.

### Advantages

- native Azure deployment format,
- complete Azure feature coverage.

### Reasons Rejected

ARM JSON templates are:

- verbose,
- difficult to maintain,
- difficult to review,
- harder to modularise.

Bicep compiles to ARM while providing substantially improved developer ergonomics.

---

## Terraform

### Description

Use Terraform for multi-cloud Infrastructure as Code.

### Advantages

- cloud portability,
- mature ecosystem,
- broad provider support.

### Reasons Rejected

The project is intentionally Azure-focused rather than cloud-agnostic.

Bicep provides:

- tighter Azure integration,
- simpler Azure resource modelling,
- native ARM compatibility,
- reduced abstraction overhead.

Terraform may still be appropriate in genuinely multi-cloud environments.

---

## Pulumi

### Description

Use general-purpose programming languages for IaC.

### Advantages

- strong language tooling,
- reusable abstractions,
- imperative logic support.

### Reasons Rejected

The project prioritises:

- declarative infrastructure,
- Azure-native tooling,
- operational simplicity,
- alignment with common Azure enterprise practices.

---

# Implementation Notes

## Repository Structure

Recommended structure:

```text
infrastructure/
  modules/
  environments/
  pipelines/
```

---

## Module Structure

Example modules:

```text
modules/
  app-service.bicep
  cosmosdb.bicep
  sql.bicep
  servicebus.bicep
  keyvault.bicep
  monitoring.bicep
```

---

## Environment Parameters

Environment-specific values should be externalised.

Example:

```text
main.dev.bicepparam
main.test.bicepparam
main.prod.bicepparam
```

---

## Validation

Infrastructure validation should include:

```bash
az bicep build --file main.bicep
```

and:

```bash
az deployment group what-if
```

before deployment execution.

---

## Deployment Pipelines

Pipelines should support:

- validation,
- what-if analysis,
- staged deployment,
- approval gates,
- deployment observability.

---

## Secret Management

Sensitive values should not be embedded directly in Bicep templates.

Secrets should be managed using:

```text
Azure Key Vault
```

and referenced securely during deployment.

---

# Scope

This decision applies to:

- Azure infrastructure provisioning,
- environment deployment automation,
- infrastructure lifecycle management,
- CI/CD infrastructure deployment workflows.

This decision does not mandate Bicep for:

- application configuration,
- Kubernetes manifests,
- local Docker orchestration.

---

# Future Considerations

Potential future enhancements include:

- deployment stacks,
- template registries,
- policy-as-code integration,
- blueprint governance,
- multi-subscription deployment orchestration,
- reusable enterprise module libraries.

These capabilities should be introduced incrementally as deployment complexity grows.

---

# References

Related documents:

- `docs/architecture/azure-deployment.md`
- `docs/architecture/overview.md`
- `docs/adr/0001-use-event-sourcing.md`
- `docs/adr/0002-use-cosmosdb-for-event-store.md`