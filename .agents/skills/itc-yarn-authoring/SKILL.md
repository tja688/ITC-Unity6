---
name: itc-yarn-authoring
description: Author and validate .yarn dialogue scripts for the ITC project using Yarn Spinner Unity 3.x with Text Animator integration. Use when writing, editing, reviewing, or linting project .yarn files, including markup rules, events, and optional YarnMarkupConverter mode.
---

# Itc Yarn Authoring

## Overview

Write .yarn dialogue that follows the project's Yarn Spinner + Text Animator rules and validate it with the provided lint script.

## Workflow

1. Read the authoritative spec at `\references\TA_YarnSpinner_Integration_Documentation.md`.
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

<!-- ITC-YARN-AUTHORING:UPDATE-START -->
## UPDATE Area (Append-Only)

This area is the only place for capability evolution. Keep the original baseline above intact.

### Update Policy (Mandatory)

When this skill is used and a new Yarn-related capability is requested (for example music trigger, portrait animation, minigame command), you must:

1. Verify runtime truth in scripts first (do not infer from memory).
2. Add one new update entry in this section only.
3. Use a unique update id: `UPD-YYYYMMDD-XX`.
4. Include: `Why`, `Source Files`, `Added/Changed Contract`, `Authoring Rules`, `Validation`.
5. Do not rewrite history entries. Corrections must be a new entry referencing old id.
6. If baseline text conflicts with new runtime, keep baseline untouched and resolve conflict in a new update entry.

Template:

```md
#### UPD-YYYYMMDD-XX: <short title>
Why:
- <why this was added>

Source Files:
- `path/to/fileA`
- `path/to/fileB`

Added/Changed Contract:
- <new command/variable/tag behavior>

Authoring Rules:
- <how writers should write .yarn after this update>

Validation:
- <what was checked: grep/lint/runtime evidence>
```

### Capability Index (Current Runtime)

- Visual commands: `itc_bg`, `itc_npc_main`, `itc_npc_avatar`, `itc_pc_avatar`, `itc_npc_main_hide`, `itc_npc_avatar_hide`, `itc_pc_avatar_hide`
- Flow/system commands: `itc_load_scene`
- Contracting minigame commands: `itc_doc_review`, `itc_rune_typing`, `itc_stamp_select`, `itc_soul_collect`, `itc_bean_sell`, `itc_settlement`, `itc_rune_verify`
- Text Animator bridge: `|...|` leading tags, `<...>` behavior tags, `<?event...>` message events, skip-event filtering policy

#### UPD-20260221-01: Sync skill with implemented ITC dialogue runtime
Why:
- Project has moved from generic Yarn authoring to a concrete ITC runtime with portrait/background switching, minigame command chain, and scene handoff.
- Previous skill text lacked command contracts and variable I/O, causing easy route logic mismatch.

Source Files:
- `Assets/Scripts/Dialogue/ITCDialoguePanel.cs`
- `Assets/Scripts/Dialogue/DialogueVisualCatalog.cs`
- `Assets/Scripts/Dialogue/DialogueSpriteProvider.cs`
- `Assets/Scripts/Dialogue/TALinePresenter.cs`
- `Assets/Scripts/Dialogue/TADialogueEvents.cs`
- `Assets/Scripts/Dialogue/YarnMarkupConverter.cs`
- `Assets/Scripts/SignMiniGame/Common/SignMiniGameFlowStateModel.cs`
- `Assets/Scripts/SignMiniGame/Common/SignMiniGameCommands.cs`
- `Assets/Doc/ITC Doc/dialogue/ITC_YARN_DATA_TEXT_ANIM.yarn`

Added/Changed Contract:
- Registered Yarn commands are currently hard-bound in `ITCDialoguePanel.RegisterCommands()`:
  - `<<itc_bg key>>`
  - `<<itc_npc_main key>>`
  - `<<itc_npc_avatar key>>`
  - `<<itc_pc_avatar key>>`
  - `<<itc_npc_main_hide>>`
  - `<<itc_npc_avatar_hide>>`
  - `<<itc_pc_avatar_hide>>`
  - `<<itc_load_scene sceneName>>`
  - `<<itc_doc_review clientId>>`
  - `<<itc_rune_typing clientId gridSize>>` (`gridSize` clamped to 4..5)
  - `<<itc_stamp_select clientId>>`
  - `<<itc_soul_collect clientId targetPercent>>` (`targetPercent` clamped to 0..100 then runtime range)
  - `<<itc_bean_sell clientId>>`
  - `<<itc_settlement clientId>>`
  - `<<itc_rune_verify clientId>>`
- Command-variable contract (writer-facing):
  - `itc_doc_review` writes `$Route_DocReviewResult`, `$Sign_mistake`, `$satisfaction`
  - `itc_rune_typing` writes `$Route_QTEErrorCount`
  - `itc_stamp_select` writes `$Route_StampType`, `$Route_StampTimingResult`, `$Sign_mistake`, `$satisfaction`
  - `itc_soul_collect` writes `$Route_SoulCollectPercent`, `$Route_SoulMin`, `$Route_SoulMax`, `$Sign_mistake`, `$satisfaction`
  - `itc_bean_sell` writes `$Route_BeanSellResult`, `$Route_BeanSoldCount`, `$satisfaction`, `$Sign_mistake`
  - `itc_settlement` writes `$satisfaction`, `$Sign_mistake`, `$money`, `$Nmber_of_sign_mistake`, `$Global_sign_mistake`, `$Route_SettlementTip`, `$Route_SettlementTier`, `$Route_SettlementFinalSatisfaction`
  - `itc_rune_verify` writes `$Route_RuneVerifyResult`, `$Route_RuneVerifyDebuff`
- Runtime value domains confirmed in contracting commands/models:
  - `$Route_DocReviewResult`: `passed | rejected_correct | rejected_wrong`
  - `$Route_StampTimingResult`: `perfect | normal | failed`
  - `$Route_StampType`: `事件 | 特技 | 名利 | 金钱`
  - `$Route_BeanSellResult`: `success | failed | skipped`
  - `$Route_SettlementTier`: `good | neutral | bad`
  - `$Route_RuneVerifyResult`: `success | failed | skipped`
  - `$Route_RuneVerifyDebuff`: `0 | 1`
- Visual mapping behavior:
  - Visual keys resolve through `DialogueVisualCatalog` + `DialogueSpriteProvider`.
  - Missing/failed mapping attempts fallback chain: requested key -> slot default key -> missing sprite.
  - Portrait/background resources can load from bundle or editor asset path (simulation mode).
- Text Animator integration behavior:
  - Leading `|tag|` tokens are parsed by `TALinePresenter` and injected as default appearance/disappearance tags for that line.
  - `|typewriter|` is intentionally ignored in the leading-pipe parser to avoid polluting defaults.
  - Disappearance-style leading tags are recognized by `|#tag|`.
  - `<?event>` messages are bridged in `TADialogueEvents`; skip mode filter can block events by policy (`Blacklist|Whitelist|BlockAll`).

Authoring Rules:
- Before using any `itc_*` command in new `.yarn`, verify it exists in `ITCDialoguePanel.RegisterCommands()`.
- Treat route variables as strict contracts; branch only on confirmed values listed above.
- For visual keys (`itc_bg` / portrait keys), only use keys present in `DialogueVisualCatalog` mappings/default keys.
- Keep default markup mode as baseline: `<>` + `|...|`; only use converter mode when explicitly requested.
- If scene transition is needed between major route phases, use `<<itc_load_scene SceneName>>` as the runtime-supported path.

Validation:
- Searched command registrations and variable writes in dialogue/contracting scripts.
- Cross-checked with live authoring example in `ITC_YARN_DATA_TEXT_ANIM.yarn`.
- Verified lint tooling still aligns with baseline mode switching (`default` vs `converter`).

#### UPD-20260221-02: Fix skill self-check path guidance for this repo layout
Why:
- This workspace has both `.agent` and `.codex` skill mirrors; authoring instructions must prefer the current skill path for deterministic use.

Source Files:
- `.agent/skills/itc-yarn-authoring/SKILL.md`
- `.agent/skills/itc-yarn-authoring/scripts/check_yarn.ps1`

Added/Changed Contract:
- Preferred self-check command in this repo:
  - `.\.agent\skills\itc-yarn-authoring\scripts\check_yarn.ps1 -Path <file-or-dir>`
  - Converter mode: `.\.agent\skills\itc-yarn-authoring\scripts\check_yarn.ps1 -Path <file-or-dir> -Mode converter`

Authoring Rules:
- Use `.agent` path first in this repository.
- `.codex` mirror may still work, but should be treated as compatibility fallback, not canonical entry.

Validation:
- Confirmed both paths exist in workspace; set `.agent` as canonical for this skill update.

#### UPD-20260225-03: Add sign-scene dialogue routing test commands
Why:
- Sign-scene acceptance now needs explicit NPC/player routing and a placeholder minigame gate directly from Yarn, without invoking legacy minigame implementations.

Source Files:
- `Assets/Scripts/Dialogue/Sign/SignDialogueSlotRuntime.cs`
- `Assets/Tests/Test dialogue/TestMinigames.yarn`

Added/Changed Contract:
- New Yarn commands registered by sign-scene runtime:
  - `<<itc_sign_npc_enter npcId>>`: reset current NPC cycle/history and enter a new NPC phase.
  - `<<itc_sign_npc_exit>>`: hide NPC cycle display and clear current NPC history.
  - `<<itc_sign_role role>>`: set line role override (`npc | player | thought | auto`).
  - `<<itc_sign_minigame token>>`: show placeholder overlay minigame and wait for completion key before continuing.

Authoring Rules:
- For sign-scene test scripts, use `itc_sign_npc_enter` when switching active NPC so Slot2 history scope resets deterministically.
- Use `itc_sign_role player/thought` before player/inner-monologue lines and return with `itc_sign_role auto` afterward.
- Use `itc_sign_minigame <token>` for blocking-flow placeholder verification instead of legacy gameplay commands during test-stage acceptance.

Validation:
- Updated and linted `Assets/Tests/Test dialogue/TestMinigames.yarn` via:
  - `.\.agent\skills\itc-yarn-authoring\scripts\check_yarn.ps1 -Path "Assets/Tests/Test dialogue/TestMinigames.yarn"`

<!-- ITC-YARN-AUTHORING:UPDATE-END -->
