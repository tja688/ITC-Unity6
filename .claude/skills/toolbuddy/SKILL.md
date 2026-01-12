---
name: toolbuddy
description: ToolBuddy Curvy Splines and Arrays Pooling plugin for Unity. Use when working with Assets/Plugins/ToolBuddy to configure Curvy splines, control points, connections, controllers, Curvy Generator graphs/modules, or array pools settings, and when you need to understand or change their parameters and runtime behavior.
---

# ToolBuddy (Curvy + Arrays Pooling)

## Overview

Use this skill to map requests to ToolBuddy plugin components and their parameters. It covers Curvy splines, controllers, generators, connections, and array pooling settings present in this repo.

## Quick Start

1. Identify the component or module name (CurvySpline, CurvySplineSegment, CurvyController, CurvyGenerator, ArrayPoolsSettings).
2. Open the matching reference file:
   - references/curvy-global-manager.md
   - references/curvy-spline.md
   - references/curvy-control-point.md
   - references/controllers.md
   - references/generator.md
   - references/generator-modules.md
   - references/arrays-pooling.md
   - references/connections.md
3. If the request is about a CG module, open its class in `Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/CG Modules`.

## Mapping Rules

- Use serialized fields and inspector attributes as the source of truth for configuration.
- For runtime behavior, read the component's Update/Refresh/Initialize methods.
- If a parameter is not in references, locate it in source:
  - Use rg for the class name or [SerializeField].
  - Confirm defaults in CurvySplineDefaultValues or field initializers.

## Common Tasks

- Configure spline behavior: use references/curvy-spline.md for interpolation, orientation, caching, pooling, update mode, and events.
- Configure control points: use references/curvy-control-point.md for Bezier handles, TCB overrides, swirl, orientation anchors, and connections.
- Move objects along splines or CG paths: use references/controllers.md.
- Build geometry via Curvy Generator: use references/generator.md and references/generator-modules.md.
- Tune pooling and allocations: use references/arrays-pooling.md.
