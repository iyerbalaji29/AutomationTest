# Test Reporting & Azure DevOps Pipeline Integration

## Quick Reference

This document provides a quick reference for the enhanced test reporting and Azure DevOps pipeline features.

## Running Tests Locally with Tags

### PowerShell (Windows)
```powershell
# Run smoke tests in Chrome
.\RunTests.ps1 -Tags "smoke,chrome" -Browser "chrome"

# Run critical tests in all browsers
.\RunTests.ps1 -Tags "critical" -Browser "all"

# Run regression tests with report
.\RunTests.ps1 -Tags "regression" -OpenReport

# Run in headless mode
.\RunTests.ps1 -Tags "smoke" -Headless
```

### Bash (Linux/Mac)
```bash
# Make script executable
chmod +x RunTests.sh

# Run smoke tests
./RunTests.sh --tags "smoke,chrome" --browser "chrome"

# Run with report
./RunTests.sh --tags "critical" --open-report
```

### Command Line
```bash
# Run with specific tags
dotnet test --filter "Category=smoke&Category=chrome"

# Run critical or high priority tests
dotnet test --filter "Category=critical|Category=high"

# Run all tests except manual
dotnet test --filter "Category!=manual"
```

## Test Reporting

### Report Types Generated

1. **ExtentReports (HTML)** - `UITests/TestReports/TestReport_*.html`
   - Interactive dashboard
   - Screenshots on failure
   - Step-by-step execution logs

2. **TRX Reports** - `UITests/TestResults/*.trx`
   - Visual Studio format
   - Azure DevOps integration

3. **Code Coverage** - `UITests/TestResults/**/coverage.cobertura.xml`
   - Line and branch coverage

4. **JSON Summary** - `UITests/TestResults/summary_*.json`
   - Machine-readable results

5. **Markdown Summary** - `UITests/TestResults/summary_*.md`
   - Human-readable summary

### Viewing Reports

**ExtentReports:**
```bash
# Windows
start UITests\TestReports\TestReport_*.html

# Linux/Mac
xdg-open UITests/TestReports/TestReport_*.html
```

**Test Results:**
```bash
# Open latest TRX file in Visual Studio
code UITests/TestResults/testresults.trx
```

## Azure DevOps Pipeline

### Pipeline Files

1. **azure-pipelines.yml** - Full pipeline with parameters
2. **azure-pipelines-smoke.yml** - Quick smoke tests
3. **azure-pipelines-regression.yml** - Nightly regression

### Running Pipelines

**From Azure DevOps:**
1. Navigate to **Pipelines** → **Pipelines**
2. Select pipeline
3. Click **Run pipeline**
4. Set parameters:
   - **Test Tags**: `smoke,critical` (or empty for all)
   - **Browser**: `chrome`, `firefox`, `edge`, or `all`
   - **Environment**: `Test`, `Staging`, `Production`
5. Click **Run**

### Pipeline Parameters

**Example 1: Smoke Tests**
```yaml
testTags: 'smoke'
browser: 'chrome'
environment: 'Test'
```

**Example 2: Critical Tests All Browsers**
```yaml
testTags: 'critical'
browser: 'all'
environment: 'Staging'
```

**Example 3: All Tests**
```yaml
testTags: ''          # Empty = all tests
browser: 'chrome'
environment: 'Test'
```

## Tag Strategy

### Tag Categories

**Browser Tags (Required):**
```gherkin
@chrome
@firefox
@edge
```

**Test Type Tags:**
```gherkin
@smoke       # Quick tests (5-10 min)
@regression  # Full suite (30+ min)
@sanity      # Basic functionality
```

**Priority Tags:**
```gherkin
@critical    # Must-pass (P0)
@high        # Important (P1)
@medium      # Standard (P2)
@low         # Nice-to-have (P3)
```

**Feature Tags:**
```gherkin
@login
@checkout
@search
@navigation
@homepage
```

### Example Feature File

```gherkin
Feature: Shopping Cart

@chrome @smoke @critical @checkout
Scenario: Add item to cart
    Given I am on the product page
    When I click "Add to Cart"
    Then the cart should contain 1 item

@firefox @regression @medium @checkout
Scenario: Verify cart persistence
    Given I have items in cart
    When I logout and login again
    Then my cart should still contain the items
```

## Test Execution Examples

### Local Execution

```bash
# Smoke tests only
.\RunTests.ps1 -Tags "smoke"

# Critical tests in Chrome
.\RunTests.ps1 -Tags "critical,chrome"

# Regression in all browsers
.\RunTests.ps1 -Tags "regression" -Browser "all"

# Specific feature
.\RunTests.ps1 -Tags "checkout,smoke"
```

### Pipeline Execution

**Smoke Tests (Quick Validation):**
- Pipeline: `azure-pipelines-smoke.yml`
- Duration: ~5 minutes
- Use case: PR validation, quick checks

**Full Regression (Comprehensive):**
- Pipeline: `azure-pipelines-regression.yml`
- Duration: ~30+ minutes
- Use case: Nightly runs, release validation

**Custom Execution:**
- Pipeline: `azure-pipelines.yml`
- Duration: Variable
- Use case: On-demand testing with specific tags

## Report Analysis

### Accessing Reports in Azure DevOps

1. **Test Results Tab**
   - Pass/Fail counts
   - Test duration
   - Error messages
   - Trend analysis

2. **Code Coverage Tab**
   - Coverage percentage
   - Uncovered lines
   - File-level details

3. **Artifacts**
   - Download HTML reports
   - Access screenshots
   - Review logs

### Report Metrics

**ExtentReports Dashboard Shows:**
- Total tests executed
- Pass/Fail/Skip counts
- Success rate percentage
- Execution timeline
- Category distribution
- Environment details

**Example Summary:**
```
Total: 25 tests
Passed: ✅ 23
Failed: ❌ 2
Skipped: ⏭️ 0
Success Rate: 92%
Duration: 5m 32s
```

## CI/CD Integration

### Branch Policies

Add required pipeline run for PRs:
1. **Project Settings** → **Repositories**
2. Select repository → **Policies**
3. Add **Build validation**
4. Select `azure-pipelines-smoke.yml`
5. Set as required

### Release Gates

Use test results as release gates:
1. Create release pipeline
2. Add **Gates** to stage
3. Add **Query Work Items** gate
4. Configure to block on test failures

### Scheduled Runs

Nightly regression tests:
```yaml
schedules:
  - cron: "0 2 * * *"  # Daily at 2 AM
    displayName: 'Nightly Regression'
    branches:
      include:
        - main
    always: true
```

## Troubleshooting

### Issue: Tests not running with tags

```bash
# Check tag syntax
dotnet test --filter "Category=smoke&Category=chrome"

# Verify tags in feature file
@chrome @smoke

# Rebuild project
dotnet build
```

### Issue: Reports not generated

```csharp
// Ensure AfterTestRun hook executes
[AfterTestRun]
public static void AfterTestRun()
{
    ExtentReportManager.FlushReport();
}
```

### Issue: Pipeline fails

```yaml
# Add continueOnError
continueOnError: true

# Check logs
- View pipeline logs in Azure DevOps
- Check test output
- Review screenshots for failures
```

## Best Practices

### Tagging
✅ Every scenario should have: browser tag + type tag + priority tag
✅ Use consistent lowercase tags
✅ Group related tests with feature tags

### Reporting
✅ Always generate reports, even on failure
✅ Attach screenshots for failed tests
✅ Include environment information
✅ Archive old reports periodically

### Pipeline
✅ Run smoke tests on every PR
✅ Run full regression nightly
✅ Use parallel execution for multiple browsers
✅ Fail fast on critical test failures
✅ Publish all artifacts

## Quick Links

- **[Full Azure DevOps Guide](AZURE_DEVOPS_PIPELINE_GUIDE.md)** - Comprehensive pipeline documentation
- **[Test Reporting Guide](TEST_REPORTING_GUIDE.md)** - Detailed reporting documentation
- **[Main README](README.md)** - Framework overview
- **[Quick Start Guide](QUICK_START_GUIDE.md)** - Get started in 5 minutes

## Command Cheat Sheet

```bash
# Local test runs
.\RunTests.ps1 -Tags "smoke"
.\RunTests.ps1 -Tags "critical" -Browser "all"
.\RunTests.ps1 -Tags "regression,checkout" -OpenReport

# Direct dotnet commands
dotnet test --filter "Category=smoke&Category=chrome"
dotnet test --filter "Category=critical|Category=high"
dotnet test --logger "trx" --results-directory TestResults

# Pipeline runs
# Use Azure DevOps UI with parameters

# View reports
start UITests\TestReports\TestReport_*.html
start UITests\Screenshots\
code UITests\Logs\TestLog_*.txt
```

---

For detailed information, see the comprehensive guides mentioned above.
