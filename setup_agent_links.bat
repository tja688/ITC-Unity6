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

echo.
echo Success! Agent folders are now linked to .agent
echo You can now safely close this window.
pause
