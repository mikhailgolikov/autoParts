# DAST baseline checks (OWASP-aligned) + optional OWASP ZAP via Docker
param(
    [string]$TargetUrl = "http://localhost:5065",
    [switch]$UseZapDocker
)

$ErrorActionPreference = "Stop"
$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$ReportDir = Join-Path $Root "security\reports\dast"
New-Item -ItemType Directory -Force -Path $ReportDir | Out-Null

$findings = @()
$endpoints = @(
    "$TargetUrl/swagger/index.html",
    "$TargetUrl/api/company",
    "$TargetUrl/api/products"
)

function Add-Finding($id, $name, $risk, $url, $detail, $solution) {
    $script:findings += [PSCustomObject]@{
        pluginId = $id
        alert    = $name
        risk     = $risk
        url      = $url
        detail   = $detail
        solution = $solution
    }
}

Write-Host "=== DAST: Security baseline ($TargetUrl) ===" -ForegroundColor Cyan

foreach ($url in $endpoints) {
    try {
        $resp = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 10
        $h = $resp.Headers

        if (-not $h["X-Content-Type-Options"]) {
            Add-Finding "10021" "X-Content-Type-Options Header Missing" "Medium" $url `
                "Response does not include X-Content-Type-Options: nosniff" `
                "Add header X-Content-Type-Options nosniff in SecurityHeadersExtensions.cs"
        }
        if (-not $h["X-Frame-Options"] -and -not ($h["Content-Security-Policy"] -match "frame-ancestors")) {
            Add-Finding "10020" "X-Frame-Options Header Missing" "Medium" $url `
                "Clickjacking protection header is absent" `
                "Add X-Frame-Options DENY or CSP frame-ancestors none"
        }
        if (-not $h["Content-Security-Policy"]) {
            Add-Finding "10038" "Content Security Policy Header Missing" "Medium" $url `
                "No CSP header, XSS impact is higher if injection exists" `
                "Add restrictive Content-Security-Policy for API responses"
        }
        if (-not $h["Referrer-Policy"]) {
            Add-Finding "10035" "Referrer Policy Header Missing" "Low" $url `
                "Referrer-Policy not set" `
                "Add Referrer-Policy strict-origin-when-cross-origin"
        }
        if ($url -like "https://*" -and -not $h["Strict-Transport-Security"]) {
            Add-Finding "10035" "Strict-Transport-Security Header Missing" "Medium" $url `
                "HSTS not enabled on HTTPS" `
                "Add Strict-Transport-Security with max-age 31536000"
        }
        if ($h["Server"]) {
            Add-Finding "10036" "Server Leaks Version Information" "Low" $url `
                "Server header exposes technology stack" `
                "Remove or genericize Server header in production"
        }
    } catch {
        Add-Finding "90022" "Unreachable Endpoint" "Info" $url $_.Exception.Message "Ensure API is running"
    }
}

try {
    Invoke-WebRequest -Uri "$TargetUrl/api/nonexistent-endpoint-xyz" -UseBasicParsing -ErrorAction Stop | Out-Null
} catch {
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $body = $reader.ReadToEnd()
        if ($body -match "stack|exception|trace") {
            Add-Finding "90022" "Application Error Disclosure" "Medium" "$TargetUrl/api/nonexistent-endpoint-xyz" `
                "Error response may leak stack trace details" `
                "Use generic error pages in production"
        }
    }
}

$JsonReport = Join-Path $ReportDir "dast-baseline-report.json"
$MdReport = Join-Path $ReportDir "dast-baseline-report.md"

$report = @{
    target    = $TargetUrl
    scannedAt = (Get-Date).ToUniversalTime().ToString("o")
    tool      = "OWASP-aligned baseline (ZAP rule IDs referenced)"
    findings  = $findings
    summary   = @{
        total  = $findings.Count
        medium = ($findings | Where-Object { $_.risk -eq "Medium" }).Count
        low    = ($findings | Where-Object { $_.risk -eq "Low" }).Count
    }
}

$report | ConvertTo-Json -Depth 5 | Set-Content $JsonReport -Encoding UTF8

$md = @()
$md += "# DAST Baseline Report"
$md += ""
$md += "**Target:** $TargetUrl"
$md += "**Date:** $(Get-Date -Format u)"
$md += ""
$md += "## Findings ($($findings.Count))"
$md += ""
foreach ($f in $findings) {
    $md += "### [$($f.pluginId)] $($f.alert) - $($f.risk)"
    $md += "- URL: $($f.url)"
    $md += "- Detail: $($f.detail)"
    $md += "- Solution: $($f.solution)"
    $md += ""
}
$md | Set-Content $MdReport -Encoding UTF8

Write-Host "Findings: $($findings.Count) - report: $MdReport" -ForegroundColor Yellow

if ($UseZapDocker) {
    Write-Host ""
    Write-Host "=== OWASP ZAP Docker scan ===" -ForegroundColor Cyan
    $RulesFile = Join-Path $Root "security\dast\zap-rules.conf"
    docker run --rm `
        -v "${ReportDir}:/zap/wrk:rw" `
        -v "${RulesFile}:/zap/wrk/zap-rules.conf:ro" `
        --add-host=host.docker.internal:host-gateway `
        ghcr.io/zaproxy/zaproxy:stable `
        zap-baseline.py -t $($TargetUrl.Replace("localhost", "host.docker.internal")) `
        -r zap-baseline-report.html -J zap-baseline-report.json `
        -w zap-baseline-report.md -c /zap/wrk/zap-rules.conf -I
}
