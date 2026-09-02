---
layout: page
title: Development
eyebrow: Source
description: Branching strategy and issue tracking.
permalink: /development/
---



## Branching

This repository uses **trunk-based development**. `main` is the trunk and is always expected to be releasable.

Work lands through short-lived branches and pull requests into `main`. The CI pipeline runs on those pull requests and on every push to `main`. Infrastructure, application, and UI deploys are started separately when the `dev` environment needs a new drop. See [Deployment]({{ '/deployment/' | relative_url }}).

## Issues

Open work items are tracked on GitHub Issues. This was chosen over Github Projects to keep things simple. As I am effectively a team of one - a straightforward list of requirements is sufficient.

<a href="{{ site.project.github.issues }}" target="_blank" rel="noopener noreferrer">{{ site.project.github.issues }}</a>
