---
layout: home
title: Overview
description: Pitch, live demo, and credentials for this reference project.
hide_sidebar: true
permalink: /
---

## Start here

Employers should leave this page with the problem, a working demo, and a path into architecture and decisions. Everything else is supporting evidence.

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

{% include callout.html type="note" title="Template copy" content="Replace this section when a project site is generated. Keep the pitch on the hero to two or three sentences. Do not paste a full product essay onto the home page." %}
