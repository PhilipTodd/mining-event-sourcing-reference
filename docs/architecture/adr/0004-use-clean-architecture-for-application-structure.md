---
layout: page
title: Use Clean Architecture for Application Structure
eyebrow: ADR 0004
permalink: /architecture/adr/0004-use-clean-architecture-for-application-structure/
---

## Context and Problem Statement

The Event Sourcing Reference application contains distinct domain, application, infrastructure, API, and worker/function concerns. The codebase should make those responsibilities explicit, keep the domain model independent of framework and infrastructure details, and remain understandable to reviewers assessing architectural and implementation quality.

The application structure should therefore support separation of concerns, dependency inversion, testability, and the ability to change infrastructure implementations without coupling those changes to core business logic.

## Considered Options

* Use Clean Architecture with inward-pointing dependencies
* Use a traditional layered architecture
* Use a feature-based architecture without strict dependency boundaries
* Use a single application project with logical folders only

## Decision Outcome

Chosen option: **"Use Clean Architecture with inward-pointing dependencies"**, because it provides clear boundaries between domain logic, application use cases, infrastructure concerns, and delivery mechanisms while keeping the core business model independent of Azure, database, messaging, and web-framework implementations.

The solution follows an onion-style dependency model in which outer layers depend on inner layers, while inner layers do not depend on implementation-specific infrastructure. Interfaces required by application logic are defined in inner layers and implemented by infrastructure projects.

This allows the domain and application layers to remain focused on business behaviour while infrastructure concerns such as Cosmos DB, Azure SQL, Service Bus, authentication, and hosting remain replaceable implementation details.

### Consequences

* Good, because domain logic remains independent of ASP.NET Core, Azure SDKs, persistence technologies, and other infrastructure concerns.
* Good, because dependencies flow inward, making architectural boundaries explicit and easier to review.
* Good, because application use cases can be tested without requiring real infrastructure.
* Good, because infrastructure implementations can be replaced or evolved with limited impact on the domain and application layers.
* Good, because the structure makes the responsibilities of domain, application, infrastructure, API, and projection processing easier to understand.
* Good, because the approach demonstrates practical use of dependency inversion and interface-based design.
* Bad, because the solution contains more projects, interfaces, and dependency configuration than a simpler layered or monolithic structure.
* Bad, because relatively small features can require changes across several projects.
* Bad, because excessive abstraction is possible if interfaces are introduced without a meaningful architectural boundary.
* Bad, because developers need to understand and maintain the dependency rules for the architecture to remain effective.