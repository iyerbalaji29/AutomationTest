# Azure DevOps Pipeline Guide

Comprehensive guide for running automation tests in Azure DevOps pipelines with tag-based execution and reporting.

## Table of Contents

- [Overview](#overview)
- [Pipeline Files](#pipeline-files)
- [Test Tagging Strategy](#test-tagging-strategy)
- [Running Tests with Tags](#running-tests-with-tags)
- [Pipeline Configuration](#pipeline-configuration)
- [Test Reporting](#test-reporting)
- [Troubleshooting](#troubleshooting)

## Overview

The framework includes three Azure DevOps pipeline configurations:

1. **azure-pipelines.yml** - Full pipeline with parameterized execution
2. **azure-pipelines-smoke.yml** - Quick smoke tests
3. **azure-pipelines-regression.yml** - Full regression suite

## Pipeline Files

### Main Pipeline (azure-pipelines.yml)

**Features:**
- Multi-stage execution (Build → Test → Report)
- Browser selection (Chrome, Firefox, Edge, or All)
- Tag-based test filtering
- Parallel execution support
- Comprehensive reporting
- Environment-specific testing

**Usage:**
```yaml
# Triggered by commits to main, develop, or feature branches
# Can be run manually with parameters
```

### Smoke Test Pipeline (azure-pipelines-smoke.yml)

**Features:**
- Manual trigger only
- Runs smoke tests in Chrome
- Quick feedback (5-10 minutes)
- Minimal resource usage

**Usage:**
```yaml
# Run manually from Azure DevOps
# Best for: Quick validation, PR checks
```

### Regression Pipeline (azure-pipelines-regression.yml)

**Features:**
- Scheduled execution (nightly)
- Runs all regression tests
- All browsers in parallel
- Full code coverage
- Comprehensive reporting

**Usage:**
```yaml
# Scheduled: Daily at 2 AM
# Triggered by commits to main
# Can be run manually
```

## Test Tagging Strategy

### Tag Categories

The framework uses a multi-level tagging strategy:

#### 1. Browser Tags (Required)
```gherkin
@chrome    # Run in Chrome
@firefox   # Run in Firefox
@edge      # Run in Edge
```

#### 2. Test Type Tags
```gherkin
@smoke       # Quick validation tests (5-10 min)
@regression  # Comprehensive tests (30+ min)
@sanity      # Basic functionality tests
```

#### 3. Priority Tags
```gherkin
@critical    # Must-pass tests (P0)
@high        # Important tests (P1)
@medium      # Standard tests (P2)
@low         # Nice-to-have tests (P3)
```

#### 4. Feature Tags
```gherkin
@login       # Login functionality
@checkout    # Checkout process
@search      # Search functionality
@navigation  # Navigation tests
@homepage    # Home page tests
```

#### 5. Special Tags
```gherkin
@database    # Requires database setup
@screenshot  # Takes screenshots
@performance # Performance tests
@manual      # Manual test cases
```

### Example Feature File with Tags

```gherkin
Feature: Shopping Cart Tests

@chrome @smoke @critical @checkout
Scenario: Add item to cart
    Given I am on the product page
    When I click "Add to Cart"
    Then the cart should contain 1 item

@firefox @regression @medium @checkout @database
Scenario: Verify cart persistence
    Given I have items in cart
    When I logout and login again
    Then my cart should still contain the items

@edge @smoke @high @checkout
Scenario: Remove item from cart
    Given I have 2 items in cart
    When I remove 1 item
    Then the cart should contain 1 item
```

## Running Tests with Tags

### Command Line Examples

**Run all smoke tests in Chrome:**
```bash
dotnet test --filter "Category=smoke&Category=chrome"
```

**Run critical tests in all browsers:**
```bash
dotnet test --filter "Category=critical&(Category=chrome|Category=firefox|Category=edge)"
```

**Run regression tests for checkout feature:**
```bash
dotnet test --filter "Category=regression&Category=checkout"
```

**Run high and critical priority tests:**
```bash
dotnet test --filter "Category=high|Category=critical"
```

**Exclude certain tags:**
```bash
dotnet test --filter "Category=smoke&Category!=manual"
```

### Azure DevOps Pipeline Parameters

#### Main Pipeline Parameters

**1. Test Tags Parameter**
```yaml
testTags: 'smoke,chrome'  # Run smoke tests in Chrome
testTags: 'regression'    # Run all regression tests
testTags: 'critical'      # Run only critical tests
testTags: ''              # Run all tests (empty = no filter)
```

**2. Browser Parameter**
```yaml
browser: 'chrome'    # Run in Chrome only
browser: 'firefox'   # Run in Firefox only
browser: 'edge'      # Run in Edge only
browser: 'all'       # Run in all browsers
```

**3. Environment Parameter**
```yaml
environment: 'Test'       # Test environment
environment: 'Staging'    # Staging environment
environment: 'Production' # Production environment
```

### Running Pipeline from Azure DevOps

**Step 1: Navigate to Pipelines**
1. Open your Azure DevOps project
2. Go to **Pipelines** → **Pipelines**
3. Select the pipeline you want to run

**Step 2: Run Pipeline with Parameters**
1. Click **Run pipeline**
2. Select the branch
3. Configure parameters:
   - **Test Tags**: Enter tags (e.g., `smoke,critical`)
   - **Browser**: Select browser
   - **Environment**: Select environment
4. Click **Run**

### Example Pipeline Runs

**Example 1: Smoke Tests Only**
```yaml
Parameters:
  testTags: 'smoke'
  browser: 'chrome'
  environment: 'Test'

Result: Runs all scenarios tagged with @smoke in Chrome
```

**Example 2: Critical Tests in All Browsers**
```yaml
Parameters:
  testTags: 'critical'
  browser: 'all'
  environment: 'Staging'

Result: Runs all @critical tests in Chrome, Firefox, and Edge
```

**Example 3: Checkout Feature Regression**
```yaml
Parameters:
  testTags: 'regression,checkout'
  browser: 'chrome'
  environment: 'Test'

Result: Runs checkout regression tests in Chrome
```

**Example 4: All Tests (No Filter)**
```yaml
Parameters:
  testTags: ''
  browser: 'all'
  environment: 'Test'

Result: Runs ALL tests in all browsers
```

## Pipeline Configuration

### Setting Up in Azure DevOps

**Step 1: Create Pipeline**
1. Go to **Pipelines** → **New Pipeline**
2. Select **Azure Repos Git** (or your source)
3. Select your repository
4. Choose **Existing Azure Pipelines YAML file**
5. Select `azure-pipelines.yml`
6. Click **Save and Run**

**Step 2: Configure Variables**

Add these variables in Azure DevOps:

| Variable | Value | Description |
|----------|-------|-------------|
| buildConfiguration | Release | Build configuration |
| dotnetVersion | 8.x | .NET SDK version |
| ENVIRONMENT | Test/Staging/Production | Target environment |

**Step 3: Configure Service Connections**

If testing against protected environments:
1. Go to **Project Settings** → **Service connections**
2. Add necessary connections
3. Grant pipeline access

### Pipeline Stages

#### Stage 1: Build
- Restore NuGet packages
- Build solution
- Publish build artifacts

#### Stage 2: Test
- Download build artifacts
- Install .NET SDK
- Set environment variables
- Run tests (with tag filter)
- Publish test results
- Publish code coverage
- Copy reports, screenshots, logs
- Publish test artifacts
- Calculate test statistics

#### Stage 3: Report
- Download test artifacts
- Generate consolidated report
- Publish final report

## Test Reporting

### Report Types

#### 1. TRX Reports (Visual Studio Test Results)
- **Location**: `TestResults/*.trx`
- **Format**: XML
- **Published to**: Azure DevOps Test Results
- **Features**:
  - Test pass/fail status
  - Execution time
  - Error messages
  - Stack traces

#### 2. ExtentReports (HTML)
- **Location**: `TestReports/TestReport_*.html`
- **Format**: HTML
- **Published to**: Build Artifacts
- **Features**:
  - Interactive dashboard
  - Screenshots
  - Step-by-step execution
  - Environment info
  - Filtering and search

#### 3. Code Coverage
- **Location**: `TestResults/**/coverage.cobertura.xml`
- **Format**: Cobertura XML
- **Published to**: Azure DevOps Code Coverage
- **Features**:
  - Line coverage
  - Branch coverage
  - Method coverage

#### 4. Markdown Summary
- **Location**: `TestResults/summary_*.md`
- **Format**: Markdown
- **Published to**: Build Artifacts
- **Features**:
  - Quick summary
  - Success rate
  - Pass/Fail counts
  - Execution time

#### 5. JSON Summary
- **Location**: `TestResults/summary_*.json`
- **Format**: JSON
- **Published to**: Build Artifacts
- **Features**:
  - Machine-readable format
  - Integration with other tools
  - API consumption

### Accessing Reports

**Test Results Tab**
1. Open your pipeline run
2. Click **Tests** tab
3. View pass/fail status
4. Filter by outcome, duration, etc.
5. See error messages and stack traces

**Artifacts**
1. Open your pipeline run
2. Click **Artifacts** (top right)
3. Download:
   - **TestReports** - HTML reports
   - **Screenshots** - Failure screenshots
   - **Logs** - Execution logs
   - **ConsolidatedReport** - Summary report

**Code Coverage**
1. Open your pipeline run
2. Click **Code Coverage** tab
3. View coverage metrics
4. Download coverage reports

### Sample Report Output

**Console Output:**
```
Starting scenario: Verify Angular home page loads successfully
Initialized Chrome browser
Navigating to: https://angular.io
Waiting for Angular page to load...
Angular page loaded successfully
PASS: The Angular logo should be displayed
PASS: Page title should contain "Angular"
Scenario passed: Verify Angular home page loads successfully
```

**Test Summary:**
```
Total Tests: 25
Passed: ✅ 23
Failed: ❌ 2
Skipped: ⏭️ 0
Success Rate: 92%
Execution Time: 5m 32s
```

### Customizing Reports

**ExtentReports Configuration:**

Edit `UITests/Reporting/ExtentReportManager.cs`:

```csharp
htmlReporter.Config.DocumentTitle = "Your Custom Title";
htmlReporter.Config.ReportName = "Your Report Name";
htmlReporter.Config.Theme = Theme.Dark; // or Theme.Standard

// Add custom system info
_extent.AddSystemInfo("Version", "1.0.0");
_extent.AddSystemInfo("Sprint", "Sprint 10");
```

## Troubleshooting

### Common Issues

**Issue: Tests not running with specific tags**

**Solution:**
```bash
# Verify tags in feature file
@chrome @smoke

# Check filter syntax (use Category=)
--filter "Category=smoke&Category=chrome"

# Rebuild project
dotnet build
```

**Issue: Pipeline fails but tests pass**

**Solution:**
```yaml
# Add continueOnError to test task
continueOnError: true

# Check failTaskOnFailedTests setting
failTaskOnFailedTests: false
```

**Issue: Reports not generated**

**Solution:**
```bash
# Check TestResults directory exists
# Verify logger parameter
--logger "trx;LogFileName=testresults.trx"

# Ensure PublishTestResults task is present
```

**Issue: Screenshots not attached to reports**

**Solution:**
```csharp
// Verify screenshot path is stored in ScenarioContext
_scenarioContext["ScreenshotPath"] = screenshotPath;

// Check ReportingHooks.cs attaches screenshots
ExtentReportManager.AttachScreenshot(_currentTest, screenshotPath);
```

### Pipeline Optimization

**Parallel Execution:**
```yaml
strategy:
  maxParallel: 3  # Run 3 browsers in parallel
  matrix:
    Chrome:
      browserTag: 'chrome'
    Firefox:
      browserTag: 'firefox'
    Edge:
      browserTag: 'edge'
```

**Faster Builds:**
```yaml
# Cache NuGet packages
- task: Cache@2
  inputs:
    key: 'nuget | "$(Agent.OS)" | **/packages.lock.json'
    path: $(NUGET_PACKAGES)

# Skip unnecessary steps
condition: and(succeeded(), ne(variables['Build.Reason'], 'PullRequest'))
```

**Resource Optimization:**
```yaml
# Use appropriate VM size
pool:
  vmImage: 'windows-latest'  # Or 'ubuntu-latest' for Linux

# Set timeout
timeoutInMinutes: 60
```

## Best Practices

### Tagging Best Practices

1. **Always include browser tag**: Every scenario should have a browser tag
2. **Use multiple tags**: Combine tags for better filtering
3. **Consistent naming**: Use lowercase for tags
4. **Priority tags**: Add priority to all tests
5. **Feature grouping**: Group related tests with feature tags

### Pipeline Best Practices

1. **Start with smoke tests**: Run smoke tests first, then regression
2. **Use parameters**: Make pipelines flexible with parameters
3. **Parallel execution**: Run different browsers in parallel
4. **Fail fast**: Stop on critical failures
5. **Comprehensive reporting**: Always publish reports and artifacts
6. **Scheduled runs**: Set up nightly regression runs
7. **Branch policies**: Require smoke tests to pass before merge
8. **Environment variables**: Use environment-specific settings

### Reporting Best Practices

1. **Always publish**: Publish results even on failure
2. **Screenshots**: Capture screenshots on failure
3. **Detailed logs**: Enable detailed logging
4. **Archive artifacts**: Keep artifacts for historical analysis
5. **Trend analysis**: Monitor test trends over time

## Integration with Release Pipelines

### Using Test Results in Release

**Step 1: Add Test Task in Release**
```yaml
- task: PublishTestResults@2
  displayName: 'Publish Test Results'
  inputs:
    testResultsFiles: '**/TestResults/*.trx'
    mergeTestResults: true
```

**Step 2: Gate Release on Test Results**
1. Go to **Pipelines** → **Releases**
2. Edit your release pipeline
3. Add **Gates**
4. Add **Query Work Items** gate
5. Configure to check for test failures

**Step 3: Automated Deployment**
```yaml
- stage: Deploy
  dependsOn: Test
  condition: and(succeeded(), eq(variables['FailedTests'], '0'))
  jobs:
    - job: DeployToStaging
      steps:
        - task: AzureWebApp@1
          # Deployment steps
```

## Useful Links

- [Azure DevOps Test Plans](https://docs.microsoft.com/en-us/azure/devops/test/)
- [YAML Pipeline Schema](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema)
- [Test Filtering](https://docs.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests)
- [ExtentReports Documentation](https://extentreports.com/)

---

**For more information**, refer to the main [README.md](README.md)
