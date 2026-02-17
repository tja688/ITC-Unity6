# AGENTS.md — Unity Base (Git-first, Isolate-Own-Changes)

## 0. Scope

A compact operating guide for an AI agent working in a Unity project.
Goals: **safe changes, traceable commits, runnable project**.

Core philosophy: **不管外面怎么乱，自己搞自己的，自己提交自己的。**

---

## 1. Core Rules

* Prefer **small, reversible** changes.
* **Show evidence** whenever anything is ambiguous: `status` / `diff` / logs.
* Do not claim actions you did not actually perform.
* Only commit **your own modifications** — never bundle others' uncommitted changes.
* If in doubt, **skip and report** rather than force-resolve.

---

## 2. Version Control: Auto-Detect (Default Git)

**Detect in this order:**

1. If `.git/` exists → **Git mode**
2. Else if `.svn/` exists or `svn info` works → **SVN mode**
3. Else → **No VCS**: do not claim commits; keep changes minimal and report diffs.

---

## 3. Git Mode (Isolate Own Changes)

### 3.0 Guiding Policy (Read This First)

**核心原则：自己搞自己的，自己提交自己的。**

* 工作区脏不脏**不管**，直接拉远端。
* 只追踪、提交**自己本次任务的修改**。
* 遇到冲突**不解决**，跳过冲突文件，提交其余部分，汇报了事。
* 绝不 `rebase`、绝不 `reset --hard`、绝不 `git add .`。

---

### 3.1 Before Work (mandatory)

#### 3.1.1 Record anchor (锚点)

Record the current HEAD **before** doing anything. This is used later to identify which files YOU changed.

```bash
git rev-parse HEAD
```

Save this hash as `$BASE` (in memory / variable).

#### 3.1.2 Pull remote (fast-forward only)

```bash
git pull --ff-only
```

* **Succeeded** → continue to work.
* **Failed** (conflict / diverged / "local changes would be overwritten") →
  **Hard Stop**: report evidence (see 3.5.1) and wait for user direction.
  Do NOT attempt rebase, merge, or stash.

> Note: dirty working tree is OK. If `pull --ff-only` can handle it, proceed; if it can't, stop.

---

### 3.2 During Work (rules)

* Keep diffs minimal and localized.
* Avoid renames/moves unless required (Unity GUID churn risk).
* Do not modify unrelated files "while you're here".
* If you must touch adjacent code for correctness, explain why in commit body.

---

### 3.3 After Work — Identify & Commit ONLY Own Changes

#### 3.3.1 Identify your own changes (diff against anchor)

Use the `$BASE` hash recorded in 3.1.1 to find files **you** changed:

```bash
git diff $BASE --name-only
git diff $BASE --stat
```

This lists every file that differs from the anchor — these are **your modifications** for this task.
Review the list; only files you intentionally changed should be committed.

#### 3.3.2 Stage only your files

```bash
git add <your-file-1> <your-file-2> ...
```

**Rules:**

* ✅ Use explicit file paths: `git add Assets/Scripts/Foo.cs Assets/Scripts/Foo.cs.meta`
* ✅ For many files, use targeted paths from the diff list in 3.3.1.
* ❌ Never `git add .` or `git add -A` — these may scoop up others' uncommitted files.
* ❌ Never stage files you did not intentionally modify.

#### 3.3.3 Commit

```bash
git commit -m "<type>(<scope>): <summary>"
```

Optional body:

* Why:
* Change:
* Verify:
* Risk/Rollback:

#### 3.3.4 Push (with conflict-skip logic)

1. **Try push:**

   ```bash
   git push
   ```

2. **Push succeeded** → done. ✅

3. **Push rejected** (remote has new commits) → enter conflict-skip flow:

   ```bash
   # a) Fetch latest
   git fetch

   # b) Find files changed on remote since your anchor
   git diff $BASE..origin/<branch> --name-only
   ```

   Compare this list against your committed files.

   * **No overlap** → safe to pull and push:

     ```bash
     git pull --ff-only
     git push
     ```

   * **Overlap exists** (some files you changed were also changed on remote) →
     these are **conflicted files**. Handle as follows:

     ```bash
     # c) Soft-reset your commit (undo commit but keep changes staged)
     git reset --soft HEAD~1

     # d) Unstage the conflicted files
     git reset HEAD -- <conflicted-file-1> <conflicted-file-2> ...

     # e) Pull latest
     git pull --ff-only

     # f) Re-commit only the non-conflicted files (still staged)
     git commit -m "<type>(<scope>): <summary> (excluding conflicted files)"

     # g) Push
     git push
     ```

   * **Report** which files were skipped:

     > "以下文件与远端有冲突，本次未提交：`FileA.cs`, `FileB.prefab`。你的本地修改仍在工作区，请手动处理。"

   * If `git pull --ff-only` still fails after all this → **Hard Stop** (see 3.5.1).

---

### 3.4 Post-Task Cleanup

* **Skipped files**: your local modifications remain in the working tree, untouched. Do NOT discard them.
* **Others' uncommitted files**: if you edited a file that was already dirty before your task, and it's not in your `$BASE` diff, leave it alone — do not stage, commit, or revert it.
* **No stash restore needed**: this workflow does not use stash.

---

### 3.5 Hard Stop Conditions & Required Report (Git)

Whenever any command fails, do NOT "handwave".
Always stop and report the following.

#### 3.5.1 Pull / Sync failed

Report:

* `git status`
* `git branch --show-current`
* `git log --oneline -5 --decorate`
* The failed command + full error output
* A short diagnosis:

  * "branch diverged; cannot ff-only" OR "local changes would be overwritten" OR "no upstream set" OR "auth blocked"

#### 3.5.2 Push failed (after conflict-skip flow)

Report:

* failed command + full error output
* `git status`
* `git diff --staged` (if anything is staged)
* List of files that were skipped due to remote conflict

#### 3.5.3 Commit failed

Report:

* failed command + full error output
* `git status`
* `git config user.name` and `git config user.email` (if identity-related)
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
* Don't rename/move assets unless required.
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
