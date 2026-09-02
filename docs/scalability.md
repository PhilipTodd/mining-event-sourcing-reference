---
layout: page
title: Scalability
eyebrow: Growth
description: Suitability of system for load, expected first bottlenecks and plan for scaling.
permalink: /scalability/
---

The application is reasonably scalable for its current scope. The write path, messaging, projection processing, and read path are already separated. This gives scope for horizontal or vertical scaling to meet any load.

- **API:** scales horizontally on App Service. It is largely stateless, so multiple instances can serve commands and queries. When necessary it can be scaled up for heavier workloads.
- **Cosmos DB event store:** scales well for append-heavy workloads, but partition-key design and hot partitions become important as event volume increases.
- **Service Bus:** decouples command processing from projection work and provides buffering during spikes.
- **Projection Function:** can scale independently from the API as message volume increases.
- **Azure SQL read model:** supports efficient query models, but will usually become the first constrained shared resource for read-heavy workloads.
- **Angular UI:** static assets are not a meaningful scalability concern compared with backend services and data stores.

## Scaling plan

| Area | Likely first limit | Plan |
| --- | --- | --- |
| App Service | CPU / instance count on B1 | Scale out, then a higher SKU |
| Data | DTU / connection pool on Basic SQL | Indexing, then a reserved SKU |
| Auth | Chatty login or session store | Cache identity, avoid per-request secrets |
| Chatty I/O | Unbounded lists or N+1 queries | Pagination and query budgets |

## Method to evaluate claims on this page

Using a dedicated load-testing tool should identify bottle-necks. A useful test would include simulating increasing concurrent users creating, approving, and querying blast plans while measuring API latency, throughput and error rates.

## Expected first bottlenecks

The first bottleneck I would expect is Azure SQL, particularly if read traffic grows substantially or projection updates become frequent. The current design centralises read-model persistence in a relational database, so query concurrency, connection-pool pressure, indexing, and write contention will matter before the stateless API itself becomes a problem.

The next likely bottleneck is the projection pipeline:

```
Cosmos event append
    ↓
Service Bus topic
    ↓
Projection Function
    ↓
Azure SQL
```

If events are produced faster than projections can be written, the Service Bus subscription backlog will grow. This is not a failure as the queue provides buffering but delay in projections will result in stale information in the projection store i.e. plan status may be incorrect when queried. Increasing function concurrency can improve throughput but may produce issues when multiple function instances are projecting (updating) a plan simultaneously.

## Areas to monitor

Early indicators may be:
- Service Bus subscription active message count
- Projection Function execution duration
- SQL CPU, DTU/vCore utilisation, query duration, connection pressure and blocking
- Cosmos RU consumption, throttling (429) and partition distribution
- API response time, request rate and failure rate

