---
name: unity-webgl
description: Plan and self-check Unity WebGL changes with a two-tier decision model (BLOCKER and WARNING). Use when Codex needs to design implementation plans, review code, validate WebGL compatibility, inspect build/deploy/security settings, or run risk-focused checks before merging Unity 2022.3 WebGL work.
---

# Unity WebGL

## Overview

Use this skill to make consistent planning and review decisions for Unity WebGL work.
Apply the rule matrix, run the scanner, and produce clear blocker/warning outcomes with concrete next actions.

## Workflow Decision Tree

1. Determine task mode.
- Use Planning Mode when proposing architecture, APIs, or implementation steps.
- Use Review Mode when checking an existing branch or diff.
2. Load only required references.
- Runtime constraints: `references/core-constraints.md`
- Build, deploy, security: `references/build-deploy-security.md`
- Rule IDs and decision outcomes: `references/rules-and-decision-matrix.md`
- Current repo snapshot: `references/project-baseline-itc-webgl.md`
3. Run scanner when code or settings are in scope.
- Command: `python skills/unity-webgl/scripts/webgl_self_check.py --repo-root .`
4. Publish result in two tiers.
- `BLOCKER`: must be resolved before merge.
- `WARNING`: may merge with explicit acceptance and follow-up.

## Planning Mode Procedure

1. Read `references/core-constraints.md` and `references/rules-and-decision-matrix.md`.
2. Identify which rules are likely affected by the proposed design.
3. Validate assumptions against `references/project-baseline-itc-webgl.md`.
4. Produce a design recommendation with:
- affected rules by ID.
- expected severity (`BLOCKER` or `WARNING`) if violated.
- preferred implementation path and fallback path.
5. If design touches build/deploy/runtime hosting, load `references/build-deploy-security.md` and include required header/config changes.

## Review and Self-Check Procedure

1. Run scanner with default exclusions.
- `python skills/unity-webgl/scripts/webgl_self_check.py --repo-root . --json-out Temp/unity-webgl-check.json --md-out Temp/unity-webgl-check.md`
2. Review findings by severity.
3. Map each finding to rule details in `references/rules-and-decision-matrix.md`.
4. Decide outcome.
- Any unresolved `BLOCKER` means do not approve.
- `WARNING` findings require explicit risk note and remediation plan.
5. Return output using the contract below.

## Severity Policy

| Severity | Decision impact | Requirement |
|---|---|---|
| BLOCKER | Stop merge or stop plan approval | Provide fix before continue |
| WARNING | Allow with conditions | Record risk, owner, and follow-up |

## Output Contract

Use this response shape:

1. `Scope`
- What was checked (paths, settings, assumptions).
2. `Findings`
- Group by `BLOCKER`, then `WARNING`.
- For each item: `rule_id`, `path:line` if available, short evidence, required action.
3. `Decision`
- `APPROVE`, `APPROVE_WITH_RISKS`, or `REJECT`.
- Keep decision consistent with severity policy.
4. `Next actions`
- Ordered list with owners and sequence.

## Load References Only When Needed Map

1. Always load:
- `references/rules-and-decision-matrix.md`
2. Load for code/runtime design:
- `references/core-constraints.md`
3. Load for build/release/server discussions:
- `references/build-deploy-security.md`
4. Load for repo-specific decisions:
- `references/project-baseline-itc-webgl.md`
5. Run scanner for any review or implementation-impacting plan:
- `scripts/webgl_self_check.py`
