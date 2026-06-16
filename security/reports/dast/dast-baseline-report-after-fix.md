# DAST Baseline Report

**Target:** http://localhost:5065
**Date:** 2026-06-15 19:59:56Z

## Findings (7)

### [10021] X-Content-Type-Options Header Missing - Medium
- URL: http://localhost:5065/swagger/index.html
- Detail: Response does not include X-Content-Type-Options: nosniff
- Solution: Add header X-Content-Type-Options nosniff in SecurityHeadersExtensions.cs

### [10020] X-Frame-Options Header Missing - Medium
- URL: http://localhost:5065/swagger/index.html
- Detail: Clickjacking protection header is absent
- Solution: Add X-Frame-Options DENY or CSP frame-ancestors none

### [10038] Content Security Policy Header Missing - Medium
- URL: http://localhost:5065/swagger/index.html
- Detail: No CSP header, XSS impact is higher if injection exists
- Solution: Add restrictive Content-Security-Policy for API responses

### [10035] Referrer Policy Header Missing - Low
- URL: http://localhost:5065/swagger/index.html
- Detail: Referrer-Policy not set
- Solution: Add Referrer-Policy strict-origin-when-cross-origin

### [10036] Server Leaks Version Information - Low
- URL: http://localhost:5065/swagger/index.html
- Detail: Server header exposes technology stack
- Solution: Remove or genericize Server header in production

### [10036] Server Leaks Version Information - Low
- URL: http://localhost:5065/api/company
- Detail: Server header exposes technology stack
- Solution: Remove or genericize Server header in production

### [10036] Server Leaks Version Information - Low
- URL: http://localhost:5065/api/products
- Detail: Server header exposes technology stack
- Solution: Remove or genericize Server header in production

