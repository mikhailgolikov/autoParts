# SAST: Semgrep static analysis
$ErrorActionPreference = "Stop"
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$ReportDir = Join-Path $Root "security\reports\sast"
New-Item -ItemType Directory -Force -Path $ReportDir | Out-Null

$PySemgrep = Get-ChildItem "$env:LOCALAPPDATA\Packages\PythonSoftwareFoundation.Python.3.11*\LocalCache\local-packages\Python311\Scripts\pysemgrep.exe" -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
if (-not $PySemgrep) {
    Write-Host "Installing semgrep..."
    python -m pip install --user semgrep --quiet
    $PySemgrep = Get-ChildItem "$env:LOCALAPPDATA\Packages\PythonSoftwareFoundation.Python.3.11*\LocalCache\local-packages\Python311\Scripts\pysemgrep.exe" | Select-Object -First 1 -ExpandProperty FullName
}

Write-Host "=== SAST: Semgrep ===" -ForegroundColor Cyan
$Config = Join-Path $Root "security\sast\.semgrep.yml"

& $PySemgrep scan --config $Config --config p/secrets --config p/csharp --no-git-ignore src security/samples `
    --json --output (Join-Path $ReportDir "semgrep-report.json") 2>&1 | Tee-Object (Join-Path $ReportDir "semgrep-report.txt")

& $PySemgrep scan --config $Config --config p/secrets --sarif --output (Join-Path $ReportDir "semgrep-report.sarif") src security/samples 2>&1 | Out-Null

Write-Host "Reports: $ReportDir" -ForegroundColor Green
