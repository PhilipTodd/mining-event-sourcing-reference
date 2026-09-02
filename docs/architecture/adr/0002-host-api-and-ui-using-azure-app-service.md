---
layout: page
title: Host the API and UI Using Azure App Service
eyebrow: ADR 0002
permalink: /architecture/adr/0002-host-api-and-ui-using-azure-app-service/
---

## Context and Problem Statement

The Event Sourcing Reference application requires managed hosting for both the ASP.NET Core API and the Angular web application. The hosting approach should demonstrate practical Azure deployment patterns while remaining simple to operate, cost-conscious, and appropriate for a reference application that does not require complex container orchestration.

The selected hosting model should also integrate cleanly with the shared Azure platform, CI/CD pipelines, Application Insights, and Microsoft Entra ID.

## Considered Options

* Host the API and UI using Azure App Service
* Host the application using Azure Container Apps
* Host the application using Azure Kubernetes Service
* Host the Angular UI using Azure Static Web Apps and the API separately

## Decision Outcome

Chosen option: **"Host the API and UI using Azure App Service"**, because App Service provides a managed Azure PaaS hosting model that is well suited to the application's scale and operational requirements without introducing unnecessary container-orchestration complexity.

Both applications can run on the shared App Service Plan, reducing hosting cost while still demonstrating production-relevant capabilities such as deployment slots, horizontal scaling, configuration management, TLS, authentication integration, and Application Insights monitoring.

The ASP.NET Core API is hosted as a Linux App Service. The Angular application is built into static assets and deployed to a separate Linux App Service.

### Consequences

* Good, because App Service provides a fully managed hosting environment with minimal infrastructure administration.
* Good, because the API and UI can share the existing App Service Plan, reducing the cost of keeping multiple reference applications deployed.
* Good, because App Service integrates directly with Azure DevOps deployment tasks and supports straightforward CI/CD.
* Good, because built-in Azure capabilities such as Application Insights, TLS, custom domains, scaling, configuration, and managed identity are readily available.
* Good, because the hosting architecture remains easy for reviewers to understand and keeps the focus on the application's event-sourcing and distributed-system design.
* Good, because Linux App Service provides an appropriate runtime for the .NET API while remaining cost-effective.
* Bad, because App Service provides less infrastructure control and portability than a container-orchestration platform.
* Bad, because the UI is hosted on a general-purpose App Service even though it consists primarily of static assets and could be hosted more efficiently using a dedicated static-site service.
* Bad, because applications sharing the same App Service Plan also share the underlying compute capacity and can therefore contend for resources.
* Bad, because the chosen approach does not demonstrate Kubernetes or advanced container-orchestration capabilities.