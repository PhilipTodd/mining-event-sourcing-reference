---
layout: page
title: Azure resources
eyebrow: Infrastructure
description: Services, tiers, purpose, and the Bicep (or Terraform) that creates them.
permalink: /azure/
---

{% include azure-table.html %}

{% include callout.html type="info" title="No click-ops" content="Tiers in this table must match the IaC parameters. If a SKU changes, change the Bicep or Terraform first, then this table." %}

## What to keep honest

- **Tier** is the SKU a hiring manager can price, not a marketing name.
- **Purpose** is one line. If a resource needs a paragraph, it belongs in an ADR.
- **IaC** is the module path, so a reviewer can jump from the table into source.
