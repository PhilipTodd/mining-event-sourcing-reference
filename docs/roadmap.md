---
layout: page
title: Roadmap
eyebrow: Next
description: Proposed enhancements in Now / Next / Later. This is intent, not a contract.
permalink: /roadmap/
---

<ol class="pipeline" style="--pipeline-steps: {{ site.data.roadmap.items.size }}">
  {% for item in site.data.roadmap.items %}
    <li class="pipeline__stage">
      <span class="pipeline__index">{{ forloop.index }}</span>
      <span class="pipeline__name">{{ item.title }}</span>
      <span class="pipeline__detail">{{ item.detail }}</span>
    </li>
  {% endfor %}
</ol>

{% include callout.html type="note" title="" content="These are high-level enhancements which I feel would improve the application as a portfolio item designed to illustrate my technical ability. They are not a list of steps required to make the system production ready." %}
