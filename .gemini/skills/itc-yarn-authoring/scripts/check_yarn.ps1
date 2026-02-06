param(
    [Parameter(Mandatory = $true)]
    [string]$Path,
    [ValidateSet("default", "converter")]
    [string]$Mode = "default",
    [switch]$Strict
)

$scriptPath = Join-Path $PSScriptRoot "yarn_lint.py"

if (-not (Test-Path $scriptPath)) {
    Write-Error "Missing lint script: $scriptPath"
    exit 1
}

$lintArgs = @($scriptPath, "--mode", $Mode)
if ($Strict) {
    $lintArgs += "--strict"
}
$lintArgs += $Path

$python = Get-Command python -ErrorAction SilentlyContinue
if ($python) {
    & $python.Source @lintArgs
    exit $LASTEXITCODE
}

$uv = Get-Command uv -ErrorAction SilentlyContinue
if ($uv) {
    & $uv.Source "run" "python" @lintArgs
    exit $LASTEXITCODE
}

Write-Error "Neither python nor uv was found on PATH."
exit 1
