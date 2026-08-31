---
layout: page
title: Architecture
eyebrow: Design
description: System Context, Container, and Component views. Diagrams are generated in Structurizr and exported here.
permalink: /architecture/
---

{% include callout.html type="info" title="Structurizr" content="Keep the model in source control. Export SVG (preferred) or PNG into `assets/diagrams/`, then point `_data/c4.yml` at those files." %}

{% include c4-diagrams.html %}

## How to read these views

- **System Context** - who uses it and which external systems it depends on.
- **Container** - the deployable pieces (web, API, data, workers) and their protocols.
- **Component** - the main internals of the primary container, not a class diagram.

Replace the placeholder diagrams before a project site is published.
