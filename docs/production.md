---
layout: page
title: Production readiness
eyebrow: Operations
description: Disaster recovery, downtime communications, rollback, observation, and alerting - the gap between a demo and a service.
permalink: /production/
---

{% include callout.html type="info" title="" content="The application is intended as a learning exercise and demonstration of my technical ability and is not production ready. This page is the list of work I feel would be necessary before allowing external users access." %}

## Implement deployment environments
Multiple environments for hosting the application. These are used by different team/users and require strict control over releases. 

The environments can be implemeted using a combination of resource groups, dedicated resources and deployment slots.

| Environment  | Deployment Decision Maker | Stability | Usage |
|:-------------:|:-------------:|  |  |
| dev      | developers     | Constant changes | Used by developers during coding iterations. Basic integration testing. |
| test      | QA lead     | Change on demand only | Rigorous testing of changes by QA team. Auto and manual testing. |
| stage      | Product manager/owner     | Iterations before prod release | QA testing against copy of prod data. Business test proposed deployment for correctness.  |
| prod      | Joint Business & Ops     | Strict deployment policy | The "live" site used by business and customers. Maximum covernance. Formal sign-off from QA and Product teams validating Stage results. |

## Shared Development Platform
- Move resources from shared instances to dedicated ones to allow better monitoring and allocation of cloud compute and storage.

## Disaster recovery

- Recovery point and recovery time objectives are not set.
- Data stores including DB's using geo-redundant backups.

## App downtime communications

- Who notices first: alert, user report, or pipeline failure.
- Status note location (README, status page, or issue template).
- Audience: demo users versus a real customer list.

## Rollback

- Default: swap back to the previous App Service slot / last known good artifact.
- Forward-fix only when rollback would lose data that cannot be restored.
- Record the command or pipeline job that performs the rollback.

## Observation and alerting

See [Observability]({{ '/observability/' | relative_url }}). Production additionally requires:

- An action group that reaches a human.
- Alerts on error, dependency failures, and saturation.
- A strict quiet-hours policy if the app is ever customer-facing.

