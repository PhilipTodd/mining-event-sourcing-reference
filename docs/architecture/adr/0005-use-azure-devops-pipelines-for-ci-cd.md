---
layout: page
title: Use Azure DevOps Pipelines for CI/CD
eyebrow: ADR 0005
permalink: /architecture/adr/0005-use-azure-devops-pipelines-for-ci-cd/
---

## Context and Problem Statement

The Event Sourcing Reference application requires automated build, test, infrastructure deployment, and application deployment workflows. Both Azure DevOps Pipelines and GitHub Actions are viable options, but the project should use a CI/CD platform that aligns closely with Azure deployment tooling and demonstrates enterprise-oriented delivery practices.

The pipeline implementation should support separate validation and deployment workflows for the API, Azure Function, Angular UI, and Bicep infrastructure.

## Considered Options

* Use Azure DevOps Pipelines
* Use GitHub Actions
* Use a combination of Azure DevOps Pipelines and GitHub Actions

## Decision Outcome

Chosen option: **"Use Azure DevOps Pipelines"**, because it provides mature integration with Azure, supports environment-based deployments and service connections, and reflects a CI/CD platform commonly used in enterprise Microsoft and Azure environments.

The application uses separate pipelines for continuous integration, API and Azure Function deployment, infrastructure deployment, and UI deployment. This keeps validation and deployment responsibilities explicit while allowing each part of the application to be deployed independently.

GitHub Actions remains a valid alternative and is used elsewhere across the broader reference-project portfolio, but Azure DevOps was selected for this application to demonstrate practical experience with both CI/CD platforms rather than standardising every project on a single tool.

### Consequences

* Good, because Azure DevOps integrates directly with Azure subscriptions, service connections, environments, and deployment tasks.
* Good, because CI, infrastructure deployment, backend deployment, and UI deployment can be separated into independently executable pipelines.
* Good, because pipeline definitions are stored as YAML alongside the application source and are therefore version controlled and reviewable.
* Good, because the approach demonstrates CI/CD practices commonly found in enterprise Azure environments.
* Good, because the wider reference-project portfolio demonstrates experience with both Azure DevOps Pipelines and GitHub Actions.
* Good, because deployment environments can provide additional controls and visibility around application releases.
* Bad, because the source repository and CI/CD platform are split across GitHub and Azure DevOps rather than being managed in a single platform.
* Bad, because contributors require access to Azure DevOps in addition to GitHub to inspect pipeline history and deployment configuration.
* Bad, because some pipeline concepts and tasks are specific to Azure DevOps and are not directly portable to GitHub Actions.
* Bad, because maintaining different CI/CD technologies across reference projects introduces some duplication in pipeline knowledge and configuration.