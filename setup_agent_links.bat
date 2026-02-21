@echo off
echo =========================================================
echo Setting up AI Agent Folder Junctions...
echo =========================================================
echo.

REM Ensure we are running in the script's directory
cd /d "%~dp0"

REM 1. Make sure .agent exists
if not exist ".agent\skills" (
    echo [ERROR] .agent\skills folder not found! Please run this script in the project root.
    pause
    exit /b
)

REM 2. Create target directories if they don't exist
if not exist ".claude" mkdir ".claude"
if not exist ".codex" mkdir ".codex"
if not exist ".gemini" mkdir ".gemini"

REM 3. Delete existing skills and workflows to replace them with links
echo Removing existing skills and workflows in .claude, .codex, .gemini (if any)...
rmdir /S /Q ".claude\skills" 2>nul
rmdir /S /Q ".claude\workflows" 2>nul

rmdir /S /Q ".codex\skills" 2>nul
rmdir /S /Q ".codex\workflows" 2>nul

rmdir /S /Q ".gemini\skills" 2>nul
rmdir /S /Q ".gemini\workflows" 2>nul

REM 4. Create Junctions
echo.
echo Creating Directory Junctions...
echo ---------------------------------------------------------
mklink /J ".claude\skills" ".agent\skills"
mklink /J ".claude\workflows" ".agent\workflows"

mklink /J ".codex\skills" ".agent\skills"
mklink /J ".codex\workflows" ".agent\workflows"

mklink /J ".gemini\skills" ".agent\skills"
mklink /J ".gemini\workflows" ".agent\workflows"
echo ---------------------------------------------------------
echo ---------------------------------------------------------

REM 5. Link Root AGENTS.md to respective files
echo.
echo Creating Hard Links for Agent Markdown files...
echo ---------------------------------------------------------
if exist "AGENTS.md" (
    del /Q ".agent\AGENTS.md" 2>nul
    del /Q ".claude\CLAUDE.md" 2>nul
    del /Q ".codex\AGENTS.md" 2>nul
    del /Q ".gemini\GEMINI.md" 2>nul

    mklink /H ".agent\AGENTS.md" "AGENTS.md"
    mklink /H ".claude\CLAUDE.md" "AGENTS.md"
    mklink /H ".codex\AGENTS.md" "AGENTS.md"
    mklink /H ".gemini\GEMINI.md" "AGENTS.md"
) else (
    echo [WARNING] AGENTS.md not found in the root directory.
)
echo ---------------------------------------------------------

REM 6. Update .gitignore
echo.
echo Checking .gitignore...
echo ---------------------------------------------------------
if exist ".gitignore" (
    findstr /C:".claude/skills/" ".gitignore" >nul
    if errorlevel 1 (
        echo Adding AI Agent ignore rules to .gitignore...
        echo.>>".gitignore"
        echo # AI Agent Folder Junctions>>".gitignore"
        echo .claude/skills/>>".gitignore"
        echo .claude/workflows/>>".gitignore"
        echo .codex/skills/>>".gitignore"
        echo .codex/workflows/>>".gitignore"
        echo .gemini/skills/>>".gitignore"
        echo .gemini/workflows/>>".gitignore"
        echo.>>".gitignore"
        echo # AI Agent Specific Markdown hardlinks>>".gitignore"
        echo .agent/AGENTS.md>>".gitignore"
        echo .claude/CLAUDE.md>>".gitignore"
        echo .codex/AGENTS.md>>".gitignore"
        echo .gemini/GEMINI.md>>".gitignore"
        echo Done.
    ) else (
        echo .gitignore already contains the ignore rules.
    )
) else (
    echo [INFO] .gitignore not found. Creating one with ignore rules...
    echo # AI Agent Folder Junctions>".gitignore"
    echo .claude/skills/>>".gitignore"
    echo .claude/workflows/>>".gitignore"
    echo .codex/skills/>>".gitignore"
    echo .codex/workflows/>>".gitignore"
    echo .gemini/skills/>>".gitignore"
    echo .gemini/workflows/>>".gitignore"
    echo.>>".gitignore"
    echo # AI Agent Specific Markdown hardlinks>>".gitignore"
    echo .agent/AGENTS.md>>".gitignore"
    echo .claude/CLAUDE.md>>".gitignore"
    echo .codex/AGENTS.md>>".gitignore"
    echo .gemini/GEMINI.md>>".gitignore"
    echo Done.
)
echo ---------------------------------------------------------

echo.
echo Success! Agent folders and markdown files are now linked!
echo You can now safely close this window.
pause
