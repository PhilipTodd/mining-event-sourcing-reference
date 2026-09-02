---
layout: page
title: Deployment pipeline
eyebrow: Delivery
description: Four Azure DevOps pipelines. CI on every change to main; infrastructure, API, and UI deploy to the shared dev environment on demand.
permalink: /deployment/
---

Delivery is Azure DevOps YAML under `infra/pipelines/`. CI runs on every pull request and push to `main`. The three deploy pipelines have no branch trigger. They are started by hand and target the `event-sourcing-dev` environment. The four pipelines are independent. They are not a single 1-through-4 sequence.

{% include callout.html type="info" title="Dev only, on demand" content="There is no staging or production pipeline. Infrastructure, application, and UI deploys are manual and land in the shared `dev` environment that hosts the live demo." %}

## Pipelines

| Pipeline | YAML | Trigger | What it does |
| --- | --- | --- | --- |
| CI | [`ci.yml`]({{ site.project.github.repo }}/blob/main/infra/pipelines/ci.yml) | `main` and PRs to `main` | Restore, lint, build, Domain and Application unit tests |
| Infrastructure | [`deploy-infra-dev.yml`]({{ site.project.github.repo }}/blob/main/infra/pipelines/deploy-infra-dev.yml) | Manual | Build, validate, what-if, then deploy Bicep |
| Application | [`deploy-app-dev.yml`]({{ site.project.github.repo }}/blob/main/infra/pipelines/deploy-app-dev.yml) | Manual | Build, test, deploy API and projection Function |
| UI | [`deploy-ui-dev.yml`]({{ site.project.github.repo }}/blob/main/infra/pipelines/deploy-ui-dev.yml) | Manual | Lint, build, and deploy the Angular App Service |

The three deploy pipelines use the `sc-esr-dev` service connection and the `event-sourcing-dev` Azure DevOps environment.

### CI

Runs on `ubuntu-latest` with the .NET 10 SDK. Restores the solution, lints with `dotnet format --verify-no-changes`, builds, then runs unit tests in `tests/BlastPlanning.Domain.Tests` and `tests/BlastPlanning.Application.Tests`. It does not deploy.

{% include pipeline.html id="ci" %}

### Infrastructure

Subscription-scoped Bicep for `australiaeast`, using `infra/bicep/environments/dev/main.bicep` and `main.dev.bicepparam`. Connection strings for Cosmos, SQL, and Service Bus are supplied as pipeline secrets.

{% include pipeline.html id="infra" %}

### Application

Builds the Blast Planning API and the projection Function on .NET 10, reruns the same Domain and Application tests, then publishes zip artifacts. Deploys to `api-adt-blastplanning-dev` and `func-adt-blastplanning-dev`.

{% include pipeline.html id="app" %}

### UI

Builds the Angular app in `src/frontend/blast-planning-ui` with Node 24 (`npm ci`, `npm run lint`, production configuration). Deploys to `web-adt-blastplanning-dev`.

{% include pipeline.html id="ui" %}

## Gates

| Pipeline | Blocks progress when |
| --- | --- |
| CI | Restore, `dotnet format`, build, or Domain/Application unit tests fail |
| Infrastructure | Bicep compile, subscription validate, or what-if fails |
| Application | Unit tests fail, or the API/Function deploy or host check fails |
| UI | `npm run lint` or the Angular production build fails, or the App Service deploy or host check fails |

Deploy stages run as Azure DevOps environment jobs on `event-sourcing-dev`. CI is the automatic merge gate; the other three are started when that environment needs a new infrastructure, API, or UI drop.

## Live demo

The running environment is the outcome of these pipelines, not a sidecar.

<div class="btn-row">
  <a class="btn btn--primary" href="{{ site.project.demo.url }}" target="_blank" rel="noopener noreferrer">{{ site.project.demo.label }}</a>
</div>
