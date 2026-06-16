# SCA: dotnet vulnerable packages + optional Trivy
$ErrorActionPreference = "Stop"
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$ReportDir = Join-Path $Root "security\reports\sca"
New-Item -ItemType Directory -Force -Path $ReportDir | Out-Null

Write-Host "=== SCA: dotnet list package --vulnerable ===" -ForegroundColor Cyan
Push-Location $Root
dotnet list package --vulnerable --include-transitive 2>&1 | Tee-Object (Join-Path $ReportDir "dotnet-vulnerable-packages.txt")
dotnet list package --vulnerable --include-transitive --format json | Set-Content (Join-Path $ReportDir "dotnet-vulnerable-packages.json")
Pop-Location

Write-Host ""
Write-Host "=== SCA: demo project with known CVE package ===" -ForegroundColor Cyan
$DemoDir = Join-Path $Root "security\sca\samples\VulnerablePackageDemo"
Push-Location $DemoDir
dotnet restore 2>&1 | Out-Null
dotnet list package --vulnerable --include-transitive 2>&1 | Tee-Object (Join-Path $ReportDir "dotnet-vulnerable-demo.txt")
Pop-Location

if (Get-Command docker -ErrorAction SilentlyContinue) {
    try {
        docker info 2>&1 | Out-Null
        Write-Host ""
        Write-Host "=== SCA: Trivy filesystem scan ===" -ForegroundColor Cyan
        docker run --rm -v "${Root}:/src:ro" aquasec/trivy:latest fs --scanners vuln --severity HIGH,CRITICAL,MEDIUM /src 2>&1 | Tee-Object (Join-Path $ReportDir "trivy-report.txt")
    } catch {
        Write-Host "Docker daemon not running, skipping Trivy" -ForegroundColor Yellow
    }
}

Write-Host "Reports: $ReportDir" -ForegroundColor Green
