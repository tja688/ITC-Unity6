# agents.md — Unity Base (Minimal)

## 0. Scope
A compact operating guide for an AI agent working in a Unity project.
Goals: **safe changes, traceable commits, runnable project**.

---

## 1. Core Rules
- Prefer **small, reversible** changes.
- **Read the minimum** needed; use **CodeIntel** (Sec 7) for C# navigation before `grep`.
- If anything is ambiguous, **show evidence** (status/diff/logs) and ask.

---

## 2. Version Control: Auto-Detect (Default Git)
**Detect in this order:**
1) If `.git/` exists → **Git mode**  
2) Else if `.svn/` exists or `svn info` works → **SVN mode**  
3) Else → **No VCS**: do not claim commits; keep changes minimal and report diffs.

---

## 3. Git Mode

### 3.1 Before Work (always)
- `git status`
- `git pull --rebase` (or `git fetch` + rebase/merge per repo policy)

### 3.2 After Work (if anything changed)
- Self-check: `git status` → `git diff`
- Stage intentionally:
  - Preferred: `git add -p`
  - Or: `git add <paths>`
- Commit: `git commit`
- Push: `git push`

### 3.3 Unexpected / Unrelated Changes
Do **not** bundle unrelated edits into the task commit.
- Partial staging: `git add -p` / `git commit -p`
- Park unrelated changes:
  - `git stash push -u -m "wip: <reason>"`
  - Commit your task
  - `git stash pop` (or `apply`)

### 3.4 Conflicts / Failed Rebase / Merge
Stop and report:
- `git status`
- failed command + error
- conflicted file list
Wait for user direction unless explicitly told to resolve.

---

## 4. SVN Mode

### 4.1 Before Work
- `svn update`

### 4.2 After Work (if anything changed)
- Self-check: `svn status` → `svn diff`
- Commit:
  - `svn commit` (include add/delete if needed)

### 4.3 Conflicts / Tree Conflicts
Stop and report:
- `svn status`
- conflict details / filenames
Wait for user direction unless explicitly told to resolve.

---

## 5. Unity Workflow

### 5.1 Code vs Editor Changes
- **Code-only tasks**: edit scripts, keep changes localized.
- **Scene/Prefab/Asset wiring**:
  1) Use **Unity MCP** if available and supported.
  2) Else create a **Unity Editor script/tool** to automate.
  3) Else provide **human steps** that are reproducible.

### 5.2 Safety Defaults
- Avoid mass reimports / GUID churn.
- Don’t rename/move assets unless required.
- Prefer additive changes over destructive ones.

---

## 6. QFramework Governance (Mandatory in This Project)

### 6.1 Scripts Business Code Must Use QF Architecture
- All business code development under `Assets/Scripts/**` must follow QFramework architecture.
- For implementation details, always read and follow:
  - `./.codex/skills/qframework-architecture/SKILL.md`

### 6.2 Three Key Scenarios Must Use Corresponding QF Skills
- `res` scenario -> `./.codex/skills/qframework-reskit/SKILL.md`
- `audio` scenario -> `./.codex/skills/qframework-audiokit/SKILL.md`
- `ui` scenario -> `./.codex/skills/qframework-uikit/SKILL.md`
- If a task spans multiple scenarios, combine the relevant skills; detailed rules are defined in each `SKILL.md`.

---

## 7. Code Intelligence (CodeIntel)
- **Strategy**: Semantic search first for C#; `grep` as fallback.
- **Access**: Port/Token in `Library/CodeIntelLogs/codeintel-endpoints.json`.
- **Flow**: `health` check -> `/v1/symbols` (get `symbolId`) -> `/v1/references`.

---

## 8. Pre-Commit / Pre-Report Checklist
- What you verified (compile and use Unity MCP to verify that no errors related to your changes have appeared in the Unity logs.).
- Diff matches intent; no accidental formatting churn.
- Commit message is explicit.

---

## 9. Commit Message (Short Template)
`<type>(<scope>): <summary>`

Optional body bullets:
- Why:
- Change:
- Verify:
- Risk/Rollback:

Types: `feat`, `fix`, `refactor`, `chore`, `docs`
