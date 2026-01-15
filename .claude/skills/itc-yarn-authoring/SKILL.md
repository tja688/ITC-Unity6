---
name: itc-yarn-authoring
description: Author and validate .yarn dialogue scripts for the ITC project using Yarn Spinner Unity 3.x with Text Animator integration. Use when writing, editing, reviewing, or linting project .yarn files, including markup rules, events, and optional YarnMarkupConverter mode.
---

# Itc Yarn Authoring

## Overview

Write .yarn dialogue that follows the project's Yarn Spinner + Text Animator rules and validate it with the provided lint script.

## Workflow

1. Read the authoritative spec at `Assets/Doc/TA_YarnSpinner_Integration_Documentation.md`.
2. Decide markup mode:
   - Default (project standard): use Text Animator tags in `<>`, disallow `[]`.
   - Converter mode (rare): use Yarn `[]` tags only, disallow `<>` (see YarnMarkupConverter in the spec).
3. Write nodes with `title:`, `---`, and `===` for each block.
4. Use `{}` only for Yarn interpolation/localization: `{$var}` or `{0}`.
5. Use `|...|` for Text Animator Appearance/Disappearance (e.g. `|fade|`).
6. Use events as `<?event>` tags (default mode only).
6. Avoid literal `\n`; split lines instead or use the agreed placeholder.
7. Run the self-check script and fix any errors or warnings.

## Self-check script

Run this after editing any .yarn content.

PowerShell wrapper (auto-detects `python` or `uv`):

- Default mode: `.\.codex\skills\itc-yarn-authoring\scripts\check_yarn.ps1 -Path <file-or-dir>`
- Converter mode: `.\.codex\skills\itc-yarn-authoring\scripts\check_yarn.ps1 -Path <file-or-dir> -Mode converter`

Direct Python:

- `python .\.codex\skills\itc-yarn-authoring\scripts\yarn_lint.py --mode default <file-or-dir>`
- If `python` is missing: `uv run python .\.codex\skills\itc-yarn-authoring\scripts\yarn_lint.py --mode default <file-or-dir>`

## Script-enforced rules

- Each node must have `title:`, `---`, and `===`.
- `title:` must not be empty and must be unique within the file.
- `{}` is allowed only for `{$...}` or `{number}`.
- Text Animator Appearance/Disappearance tags use `|` (e.g. `|fade|`).
- Default mode: `[]` markup tags are disallowed.
- Converter mode: `<...>` tags are disallowed.
- Literal `\n` triggers a warning.
