# AGENTS.md — Unity Base (Git-first, Stable Workflow)

## 0. Scope

A compact operating guide for an AI agent working in a Unity project.
Goals: **safe changes, traceable commits, runnable project**, and **stable VCS automation**.

---

## 1. Core Rules

* Prefer **small, reversible** changes.
* **Show evidence** whenever anything is ambiguous: `status` / `diff` / logs.
* Do not claim actions you did not actually perform.
* Keep commits focused: **only task-related diffs** in the task commit.

---

## 2. Version Control: Auto-Detect (Default Git)

**Detect in this order:**

1. If `.git/` exists → **Git mode**
2. Else if `.svn/` exists or `svn info` works → **SVN mode**
3. Else → **No VCS**: do not claim commits; keep changes minimal and report diffs.

---

## 3. Git Mode (Stable / Noise-Resistant)

### 3.0 Guiding Policy (Read This First)

This project prioritizes **stability over clever history operations**:

* Avoid `rebase` in routine automation (too easy to fail on “noise”).
* Prefer **fast-forward only** sync.
* If the branch is diverged and cannot fast-forward: **stop and report**.

### 3.1 Before Work (always, mandatory sequence)

#### 3.1.1 Snapshot current state (evidence)

* `git status`
* `git status --porcelain`
* Optional (if needed): `git branch --show-current` and `git rev-parse --short HEAD`

#### 3.1.2 If working tree is NOT clean → isolate preexisting changes (mandatory)

If `git status --porcelain` is non-empty, do **NOT** blindly hide everything.
Goal: keep **preexisting tracked edits** from polluting the task, while allowing **useful untracked files** to remain available when needed.

**A) Default: stash tracked changes only (recommended)**
Use this when there are **staged/unstaged tracked changes**, and you do not explicitly need to hide untracked files.

1. Create a stash **without** `-u`:

   * `git stash push -m "pre-task stash (tracked only): <task-brief>"`
2. Record the stash ref:

   * `git stash list` (capture top entry)
3. Verify “clean enough for work”:

   * `git status --porcelain` should contain **only** untracked (`?? ...`) entries, or be empty.
4. Important:

   * Preexisting changes are **NOT part of this task** by default.
   * Untracked files that remain visible must **not** be added/committed unless the task explicitly creates/needs them.

**B) Escalate: stash tracked + untracked (only when necessary)**
Only use `-u` when **any** of the following is true:

* You must make the working tree totally empty to avoid tooling confusion, OR
* You expect `pull --ff-only` could fail due to “untracked working tree files would be overwritten”, OR
* There are too many untracked files and they clearly block safe operation.

Steps:

1. Create a stash that includes untracked files:

   * `git stash push -u -m "pre-task stash (with untracked): <task-brief>"`
2. Record the stash ref:

   * `git stash list` (capture top entry)
3. Verify clean:

   * `git status --porcelain` **must be empty**
4. Important:

   * If you need to keep using a particular file during the task, do **not** use `-u` unless necessary.

> Rationale: “Not mine → isolate” prevents noise from blocking sync/commit, but **untracked files can be legitimate inputs** (e.g., newly created local files you still need). Defaulting to “tracked-only stash” preserves usability while keeping the task commit clean.

#### 3.1.3 Sync with remote WITHOUT rebase (fast-forward only)

1. Fetch:

   * `git fetch --prune`
2. Update local branch with fast-forward only:

   * Preferred: `git pull --ff-only`
   * Or explicit: `git merge --ff-only @{u}` (if upstream exists)

If fast-forward fails (diverged history / no upstream / remote changed unexpectedly):

* Stop and report evidence (see 3.4).
* Do **not** attempt rebase automatically.

---

### 3.2 During Work (rules)

* Keep diffs minimal and localized.
* Avoid renames/moves unless required (Unity GUID churn risk).
* Do not modify unrelated files “while you’re here”.
* If you must touch adjacent code for correctness, explain why in commit body.

---

### 3.3 After Work (if anything changed) — MUST commit task changes

This section is designed so the agent reliably produces a task commit.

#### 3.3.1 Self-check (evidence)

* `git status`
* `git diff`
* If staged content exists: `git diff --staged`

#### 3.3.2 Stage intentionally

* Preferred (review-by-hunk): `git add -p`
* Or targeted paths: `git add <paths>`
* Avoid `git add .` unless you explicitly verified every file belongs to the task.

#### 3.3.3 Commit (mandatory if task changed anything)

If there are staged changes:

* `git commit -m "<type>(<scope>): <summary>"`

If you need a body:

* `git commit` then write:

  * Why:
  * Change:
  * Verify:
  * Risk/Rollback:

If nothing is staged but working tree has changes:

* This means you forgot to stage or staged nothing intentionally.
* Re-run staging or explain why you are leaving changes uncommitted (rare; must be explicit).

#### 3.3.4 Push (mandatory if commit created)

* `git push`

If push is blocked by policy/permission:

* Stop and report evidence (see 3.4.3).

#### 3.3.5 Restore pre-task stash (mandatory if created)

If you created a “pre-task stash” in 3.1.2:

* Preferred (safer): `git stash apply`
  (keeps stash entry in case of conflict)
* If apply succeeded cleanly and you want to drop it:

  * `git stash drop stash@{0}` (only if you are certain it’s the right one)
* Or if you explicitly want pop:

  * `git stash pop`

If conflicts happen during apply/pop:

* Stop and report evidence (see 3.4.2).
* Do not resolve unless explicitly told to.

> Important: restoring the stash happens **after** task commit/push, so preexisting work does not pollute the task commit.

---

### 3.4 Hard Stop Conditions & Required Report (Git)

Whenever any command fails, do NOT “handwave”.
Always stop and report the following.

#### 3.4.1 Sync failed (ff-only not possible)

Report:

* `git status`
* `git branch --show-current`
* `git log --oneline -5 --decorate`
* `git remote -v`
* The failed command + full error output
* A short diagnosis:

  * “branch diverged; cannot ff-only” OR “no upstream set” OR “auth blocked”

#### 3.4.2 Stash restore conflict

Report:

* `git status`
* `git diff`
* Conflict file list
* The stash entry you attempted to apply/pop (`git stash list`)
  Wait for user direction unless explicitly told to resolve.

#### 3.4.3 Commit/Push failed

Report:

* failed command + full error output
* `git status`
* `git diff --staged` (if commit failed)
* `git config user.name` and `git config user.email` (if identity-related)
* If signing might matter:

  * `git config commit.gpgsign`
  * `git config gpg.format`
    Wait for user direction unless explicitly told to fix config/hook requirements.

---

## 4. SVN Mode

### 4.1 Before Work

* `svn update`

### 4.2 After Work (if anything changed)

* Self-check: `svn status` → `svn diff`
* Commit:

  * `svn commit` (include add/delete if needed)

### 4.3 Conflicts / Tree Conflicts

Stop and report:

* `svn status`
* conflict details / filenames
  Wait for user direction unless explicitly told to resolve.

---

## 5. Unity Workflow

### 5.1 Code vs Editor Changes

* **Code-only tasks**: edit scripts, keep changes localized.
* **Scene/Prefab/Asset wiring**:

  1. Use **Unity MCP** if available and supported.
  2. Else create a **Unity Editor script/tool** to automate.
  3. Else provide **human steps** that are reproducible.

### 5.2 Safety Defaults

* Avoid mass reimports / GUID churn.
* Don’t rename/move assets unless required.
* Prefer additive changes over destructive ones.

---

## 6. QFramework Governance (Mandatory in This Project)

### 6.1 Scripts Business Code Must Use QF Architecture

* All business code development under `Assets/Scripts/**` must follow QFramework architecture.
* For implementation details, always read and follow:

  * `.agent/skills/qframework-architecture/SKILL.md`

### 6.2 Three Key Scenarios Must Use Corresponding QF Skills

* `res` scenario -> `.agent/skills/qframework-reskit/SKILL.md`
* `audio` scenario -> `.agent/skills/qframework-audiokit/SKILL.md`
* `ui` scenario -> `.agent/skills/qframework-uikit/SKILL.md`
* If a task spans multiple scenarios, combine the relevant skills; detailed rules are defined in each `SKILL.md`.

---

## 7. Code Intelligence (CodeIntel)

* **Strategy**: Semantic search first for C#; `grep` as fallback.
* **Access**: Port/Token in `Library/CodeIntelLogs/codeintel-endpoints.json`.
* **Flow**: `health` check -> `/v1/symbols` (get `symbolId`) -> `/v1/references`.

---

## 8. Pre-Commit / Pre-Report Checklist

* What you verified (compile and use Unity MCP to verify that no errors related to your changes have appeared in the Unity logs.).
* Diff matches intent; no accidental formatting churn.
* Commit message is explicit.

---

## 9. Commit Message (Short Template)

`<type>(<scope>): <summary>`

Optional body bullets:

* Why:
* Change:
* Verify:
* Risk/Rollback:

Types: `feat`, `fix`, `refactor`, `chore`, `docs`
