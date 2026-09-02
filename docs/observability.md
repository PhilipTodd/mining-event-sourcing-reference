---
layout: page
title: Observability
eyebrow: Operations
description: Achieved using Azure Monitor and Application Insights. Golden signals plus a small set of custom events.
permalink: /observability/
---

This project uses **Application Insights** (Azure Monitor) for traces, metrics, and exceptions. 

Observability is essential for investigating and preventing Production issues. To keep hosting costs down the Application Insights instance is part of the Shared Development Platform. 

{% include callout.html type="info" title="" content="NOTE: Production ready observability has not yet been implemented for this project." %}

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

Using Azure Monitor and Application Insights, the Golden Signals above are tracked through a combination of out-of-the-box telemetry tables, pre-aggregated platform metrics, and custom Kusto Query Language (KQL) queries. These can be surfaced via Azure Qorkbooks and Alert Rules to allow teams to respond to growing threats quickly.

## Custom telemetry

Below are suggested events that can be traced. Actual events TBN:

- `Demo.SessionStarted`
- `Plan.Approved` / the domain equivalent
- `Auth.Failed` with result code, never a secret
