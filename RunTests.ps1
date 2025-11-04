# PowerShell script to run automation tests locally with tag filtering
# Usage: .\RunTests.ps1 -Tags "smoke,chrome" -Browser "chrome" -Environment "Test"

param(
    [Parameter(Mandatory=$false)]
    [string]$Tags = "",

    [Parameter(Mandatory=$false)]
    [ValidateSet("chrome", "firefox", "edge", "all")]
    [string]$Browser = "chrome",

    [Parameter(Mandatory=$false)]
    [ValidateSet("Test", "Staging", "Production", "Local")]
    [string]$Environment = "Local",

    [Parameter(Mandatory=$false)]
    [switch]$GenerateReport = $true,

    [Parameter(Mandatory=$false)]
    [switch]$OpenReport = $false,

    [Parameter(Mandatory=$false)]
    [switch]$Headless = $false,

    [Parameter(Mandatory=$false)]
    [string]$Configuration = "Debug"
)

# Display banner
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Automation Test Framework - Test Runner     " -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Display configuration
Write-Host "Configuration:" -ForegroundColor Yellow
Write-Host "  Tags: $($Tags -eq '' ? 'All' : $Tags)" -ForegroundColor White
Write-Host "  Browser: $Browser" -ForegroundColor White
Write-Host "  Environment: $Environment" -ForegroundColor White
Write-Host "  Headless: $Headless" -ForegroundColor White
Write-Host "  Configuration: $Configuration" -ForegroundColor White
Write-Host ""

# Set environment variable
$env:ENVIRONMENT = $Environment

# Set current location to script directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

# Clean previous results
Write-Host "Cleaning previous test results..." -ForegroundColor Yellow
if (Test-Path "UITests\TestResults") {
    Remove-Item "UITests\TestResults" -Recurse -Force -ErrorAction SilentlyContinue
}
if (Test-Path "UITests\TestReports") {
    Remove-Item "UITests\TestReports" -Recurse -Force -ErrorAction SilentlyContinue
}
Write-Host "Cleanup complete." -ForegroundColor Green
Write-Host ""

# Build solution
Write-Host "Building solution..." -ForegroundColor Yellow
$buildResult = dotnet build --configuration $Configuration --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed! Please fix build errors and try again." -ForegroundColor Red
    exit 1
}
Write-Host "Build successful." -ForegroundColor Green
Write-Host ""

# Prepare test filter
$filter = ""
if ($Tags -ne "") {
    $tagArray = $Tags -split ","
    $filterParts = @()

    foreach ($tag in $tagArray) {
        $tag = $tag.Trim()
        $filterParts += "Category=$tag"
    }

    $filter = $filterParts -join "&"
}

# Add browser filter if not "all"
if ($Browser -ne "all") {
    if ($filter -ne "") {
        $filter = "$filter&Category=$Browser"
    } else {
        $filter = "Category=$Browser"
    }
}

# Run tests
Write-Host "Running tests..." -ForegroundColor Yellow
Write-Host "Filter: $filter" -ForegroundColor Cyan
Write-Host ""

$testArgs = @(
    "test",
    "UITests\UITests.csproj",
    "--configuration", $Configuration,
    "--no-build",
    "--logger", "trx;LogFileName=testresults.trx",
    "--logger", "console;verbosity=detailed",
    "--results-directory", "UITests\TestResults"
)

if ($filter -ne "") {
    $testArgs += "--filter"
    $testArgs += $filter
}

# Run the tests
& dotnet $testArgs

$testExitCode = $LASTEXITCODE

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan

# Parse test results
$trxFile = Get-ChildItem -Path "UITests\TestResults" -Filter "*.trx" -Recurse | Select-Object -First 1

if ($trxFile) {
    [xml]$trx = Get-Content $trxFile.FullName
    $counters = $trx.TestRun.ResultSummary.Counters

    $total = [int]$counters.total
    $passed = [int]$counters.passed
    $failed = [int]$counters.failed
    $skipped = [int]$counters.notExecuted

    $successRate = if ($total -gt 0) { [math]::Round(($passed / $total) * 100, 2) } else { 0 }

    Write-Host ""
    Write-Host "Test Execution Summary:" -ForegroundColor Yellow
    Write-Host "  Total Tests: $total" -ForegroundColor White
    Write-Host "  Passed: $passed" -ForegroundColor Green
    Write-Host "  Failed: $failed" -ForegroundColor $(if ($failed -gt 0) { "Red" } else { "Green" })
    Write-Host "  Skipped: $skipped" -ForegroundColor Gray
    Write-Host "  Success Rate: $successRate%" -ForegroundColor $(if ($successRate -ge 90) { "Green" } elseif ($successRate -ge 70) { "Yellow" } else { "Red" })
    Write-Host ""

    # Generate summary file
    $summaryPath = "UITests\TestResults\summary.txt"
    @"
Test Execution Summary
=====================

Execution Time: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Environment: $Environment
Browser: $Browser
Tags: $($Tags -eq '' ? 'All' : $Tags)

Results:
--------
Total Tests: $total
Passed: $passed
Failed: $failed
Skipped: $skipped
Success Rate: $successRate%

"@ | Out-File -FilePath $summaryPath -Encoding UTF8

    Write-Host "Summary saved to: $summaryPath" -ForegroundColor Cyan
}

# Check for HTML reports
$htmlReports = Get-ChildItem -Path "UITests\TestReports" -Filter "*.html" -Recurse -ErrorAction SilentlyContinue

if ($htmlReports -and $OpenReport) {
    Write-Host ""
    Write-Host "Opening HTML report..." -ForegroundColor Yellow
    Start-Process $htmlReports[0].FullName
}

# Display artifact locations
Write-Host ""
Write-Host "Test Artifacts:" -ForegroundColor Yellow
Write-Host "  Test Results: $(Resolve-Path 'UITests\TestResults' -ErrorAction SilentlyContinue)" -ForegroundColor White
Write-Host "  HTML Reports: $(Resolve-Path 'UITests\TestReports' -ErrorAction SilentlyContinue)" -ForegroundColor White
Write-Host "  Screenshots: $(Resolve-Path 'UITests\Screenshots' -ErrorAction SilentlyContinue)" -ForegroundColor White
Write-Host "  Logs: $(Resolve-Path 'UITests\Logs' -ErrorAction SilentlyContinue)" -ForegroundColor White

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan

# Exit with appropriate code
if ($testExitCode -ne 0 -or $failed -gt 0) {
    Write-Host "Tests FAILED!" -ForegroundColor Red
    exit 1
} else {
    Write-Host "All tests PASSED!" -ForegroundColor Green
    exit 0
}
