---
layout: home
title: Overview
description: Pitch, live demo, and credentials for this reference project.
hide_sidebar: true
permalink: /
---

## Start here

This application demonstrates an event-sourced approach to explosives blast planning within the mining industry. Simple blast plans are created using the UI. Plans follow a short lifecycle consisting of a Create --> Approve path. These events are stored in Cosmos DB and distributed via Service Bus topic to a worker process which projects status into an Azure SQL DB achieving eventual consistency.

<ul class="jump-cards">
  <li>
    <a href="{{ '/architecture/' | relative_url }}">
      <strong>Architecture</strong>
      <span>C4 context, container, and component views from Structurizr.</span>
    </a>
  </li>
  <li>
    <a href="{{ '/decisions/' | relative_url }}">
      <strong>Decision log</strong>
      <span>ADRs in Date | Status | Title format.</span>
    </a>
  </li>
  <li>
    <a href="{{ '/deployment/' | relative_url }}">
      <strong>Deployment pipeline</strong>
      <span>Linting through production, including the gated release.</span>
    </a>
  </li>
  <li>
    <a href="{{ '/production/' | relative_url }}">
      <strong>Production readiness</strong>
      <span>Rollback, communications, disaster recovery, and alerting.</span>
    </a>
  </li>
</ul>
