---
layout: page
title: Azure resources
eyebrow: Infrastructure
description: Azure infrastructure is managed by IaC (Infrastructure as Code). Bicep files defined in the infra/bicep folder manage Blast Planning specifc resources referencing resrouces on the Shared Development Platform when necessary.
permalink: /azure/
---

Infrastructure strinctly IaC only. No portal click-ops allowed. This treats infrastructure with the same operational rigor as source code. It enhances cloud infrastructure management by various means including preventing configuration drift, auditability, disaster recovery and allowing peer review for proposed changes.


The entrypoint is [`infra/bicep/environments/dev/main.bicep`]({{ site.project.github.repo }}/blob/main/infra/bicep/environments/dev/main.bicep), parameterized by [`main.dev.bicepparam`]({{ site.project.github.repo }}/blob/main/infra/bicep/environments/dev/main.dev.bicepparam). The infrastructure pipeline deploys that file to `australiaeast`.

```
infra/bicep/
  environments/dev/
    main.bicep
    main.dev.bicepparam
  modules/
    appservice.bicep
    cosmos-container.bicep
    servicebus.bicep
    storage-account.bicep
```

`main.bicep` creates the application resource group for this project and the modules below. It also declares the Shared Development Platform resources as `existing` and wires their names, IDs, and connection strings into those modules. In this way the bicep files in this proejct are responsible for the resources "owned" by this project only while deploying to resources owned by the Shared Development Platform.

## Managed by this project

These resources are implemented in `infra/bicep` and live in `rg-event-sourcing-dev`, except the Cosmos container and Service Bus topic, which are created on the shared account and namespace in `rg-platform-dev`.

{% include azure-table.html group="created" %}

## Referenced from the shared platform

Declared as `existing` in `environments/dev/main.bicep`. This project consumes them. It does not create or set their SKUs.

{% include azure-table.html group="referenced" note="Referenced as existing resources in environments/dev/main.bicep. This project does not create them." %}
