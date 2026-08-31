---
layout: page
title: Observability
eyebrow: Operations
description: Azure Monitor and Application Insights. Golden signals plus a small set of custom events.
permalink: /observability/
---

Telemetry is a product feature, not a dashboard hobby. This project uses **Application Insights** (Azure Monitor) for traces, metrics, and exceptions.

## Golden signals

<div class="panel-grid">
  <div class="panel">
    <div class="panel__header"><h3 class="panel__title">Latency</h3></div>
    <div class="panel__body"><p>Server response time (P50 / P95) on the public API and the authenticated app shell.</p></div>
  </div>
  <div class="panel">
    <div class="panel__header"><h3 class="panel__title">Errors</h3></div>
    <div class="panel__body"><p>Failed requests, dependency failures, and unhandled exceptions. Alert on burn rate, not a single 500.</p></div>
  </div>
  <div class="panel">
    <div class="panel__header"><h3 class="panel__title">Traffic</h3></div>
    <div class="panel__body"><p>Request rate by operation. Use it to distinguish quiet from broken.</p></div>
  </div>
  <div class="panel">
    <div class="panel__header"><h3 class="panel__title">Saturation</h3></div>
    <div class="panel__body"><p>CPU, memory, and queue depth on App Service (and SQL DTU / vCore where relevant).</p></div>
  </div>
</div>

## Custom telemetry

Name a few events that map to the product, not the framework. Examples to replace per project:

- `Demo.SessionStarted`
- `Checkout.Completed` / the domain equivalent
- `Auth.Failed` with result code, never a secret

{% include callout.html type="note" title="Fill from the application" content="When project content is written, list the actual custom events and the alert rules that fire on them." %}
