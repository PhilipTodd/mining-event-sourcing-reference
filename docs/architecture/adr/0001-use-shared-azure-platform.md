---
layout: page
title: Use a Shared Azure Platform for Common Infrastructure
eyebrow: ADR 0001
permalink: /architecture/adr/0001-use-shared-azure-platform/
---

## Context and Problem Statement

The Event Sourcing Reference application requires a number of Azure platform services, including an App Service Plan, Azure SQL, Cosmos DB, Service Bus, Application Insights, and Log Analytics. Provisioning dedicated instances of these services for every reference project increases Azure cost, duplicates infrastructure, and creates additional operational overhead.

The reference projects need to demonstrate realistic Azure architecture while remaining cost-effective to host and maintain. Common platform resources should therefore be shared where resource isolation is not an important characteristic of the architecture being demonstrated.

## Considered Options

* Provision dedicated Azure resources for each reference project
* Share common Azure platform resources across reference projects
* Deploy all reference projects and resources into a single resource group

## Decision Outcome

Chosen option: **"Share common Azure platform resources across reference projects"**, because it significantly reduces the cost and duplication associated with running multiple reference applications while retaining clear logical separation between application-specific and shared infrastructure.

A dedicated shared platform owns common infrastructure such as the App Service Plan, Azure SQL logical server and database, Cosmos DB account and database, Service Bus namespace, Application Insights, and Log Analytics workspace.

Each reference application continues to own its application-specific resources and logical data structures, such as App Services, Azure Functions, Cosmos DB containers, Service Bus topics and subscriptions, and application-specific SQL schemas and tables.

### Consequences

* Good, because multiple reference applications can reuse Azure resources that would otherwise be unnecessarily duplicated.
* Good, because Azure hosting costs are reduced, which is important for demonstration applications that remain deployed but receive relatively little production traffic.
* Good, because ownership boundaries remain explicit: the shared platform owns common infrastructure while each application owns its application-specific resources and configuration.
* Good, because the approach demonstrates the architectural distinction between platform infrastructure and application infrastructure.
* Good, because common platform resources can be provisioned and maintained independently from individual applications.
* Bad, because applications have less infrastructure isolation than they would with completely dedicated Azure resources.
* Bad, because changes or failures affecting a shared platform resource can potentially affect multiple reference applications.
* Bad, because resource-level performance contention is possible when multiple applications share the same underlying service.
* Bad, because deployment and deletion of individual applications must account for resource ownership to ensure shared platform resources are not inadvertently modified or removed.