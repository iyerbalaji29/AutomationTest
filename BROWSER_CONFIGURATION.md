# Browser Configuration Guide

## Overview

The test automation framework uses **RunSettings files** to configure which browser to use for test execution. The browser is specified once in the RunSettings file and applies to the entire test suite.

## Key Principle

**Browser configuration is centralized in RunSettings, NOT in individual tests.**

- ✅ All tests use the `Browser` parameter from the selected RunSettings file
- ✅ To run tests in a different browser, select a different RunSettings file
- ❌ No browser-specific test categories (e.g., `[Category("Chrome")]`)
- ❌ No browser parameterization in tests (e.g., `[TestCase(BrowserType.Chrome)]`)

## How It Works

### 1. Browser Configuration in RunSettings

Each RunSettings file specifies the browser to use:

**Chrome.runsettings:**
```xml
<Parameter name="Browser" value="Chrome" />
```

**Firefox.runsettings:**
```xml
<Parameter name="Browser" value="Firefox" />
```

**Edge.runsettings:**
```xml
<Parameter name="Browser" value="Edge" />
```

### 2. Tests Read Browser from Configuration

All test fixtures initialize the browser from the RunSettings:

```csharp
[OneTimeSetUp]
public void OneTimeSetup()
{
    _config = TestConfiguration.Instance;
    _config.InitializeFromTestContext(TestContext.Parameters);

    // Browser type comes from RunSettings
    var browserType = Enum.Parse<BrowserType>(_config.Browser, true);
    _seleniumHooks.InitializeDriver(browserType);
}
```

### 3. Single Test Suite, Multiple Browser Runs

The same test code runs in any browser - just change the RunSettings file:

```bash
# Same tests, different browsers
dotnet test --settings RunSettings/Chrome.runsettings
dotnet test --settings RunSettings/Firefox.runsettings
dotnet test --settings RunSettings/Edge.runsettings
```

## Available RunSettings Files

### Browser-Specific Files
| File | Browser | Use Case |
|------|---------|----------|
| Chrome.runsettings | Chrome | Run entire suite in Chrome |
| Firefox.runsettings | Firefox | Run entire suite in Firefox |
| Edge.runsettings | Edge | Run entire suite in Edge |

### Environment Files (with default browsers)
| File | Browser | Environment |
|------|---------|-------------|
| Dev.runsettings | Chrome | Development |
| QA.runsettings | Chrome | QA Testing |
| Prod.runsettings | Chrome | Production (headless) |

**Note:** You can edit the `Browser` parameter in environment files to change the default browser.

## Running Tests

### Single Browser Execution

```bash
# Run all tests in Chrome
dotnet test --settings RunSettings/Chrome.runsettings

# Run smoke tests in Firefox
dotnet test --settings RunSettings/Firefox.runsettings --filter "Category=Smoke"

# Run login tests in Edge
dotnet test --settings RunSettings/Edge.runsettings --filter "Category=Login"
```

### Cross-Browser Testing

To test across multiple browsers, run the test suite multiple times with different RunSettings:

**PowerShell Script:**
```powershell
# Run smoke tests in all browsers
$browsers = @("Chrome", "Firefox", "Edge")
foreach ($browser in $browsers) {
    Write-Host "Running tests in $browser..."
    dotnet test --settings "RunSettings/$browser.runsettings" --filter "Category=Smoke"
}
```

**Bash Script:**
```bash
# Run all tests in all browsers
for browser in Chrome Firefox Edge; do
    echo "Running tests in $browser..."
    dotnet test --settings "RunSettings/$browser.runsettings"
done
```

### CI/CD Pipeline Example

**Azure DevOps:**
```yaml
strategy:
  matrix:
    Chrome:
      browserSettings: 'Chrome.runsettings'
    Firefox:
      browserSettings: 'Firefox.runsettings'
    Edge:
      browserSettings: 'Edge.runsettings'

steps:
  - task: DotNetCoreCLI@2
    inputs:
      command: 'test'
      arguments: '--settings RunSettings/$(browserSettings) --filter "Category=Smoke"'
```

**GitHub Actions:**
```yaml
strategy:
  matrix:
    browser: [Chrome, Firefox, Edge]

steps:
  - name: Run Tests
    run: dotnet test --settings RunSettings/${{ matrix.browser }}.runsettings
```

## Benefits of This Approach

### 1. Simplicity
- Write tests once, run in any browser
- No browser-specific code in tests
- Clear separation of concerns

### 2. Flexibility
- Change browsers without modifying code
- Easy to add new browsers (just create new RunSettings)
- Simple environment-specific browser configuration

### 3. Performance
- Browser instance reused across tests (fast)
- Parallel execution within each browser
- Can run multiple browsers in parallel (via CI/CD)

### 4. Maintainability
- Single source of truth for browser configuration
- No duplicate test code for different browsers
- Easy to update browser settings globally

## Migration from Old Approach

### Before (Browser-Specific Tests)
```csharp
// ❌ OLD: Browser hardcoded or parameterized in tests
[TestCase(BrowserType.Chrome)]
[TestCase(BrowserType.Firefox)]
[TestCase(BrowserType.Edge)]
public void TestInDifferentBrowsers(BrowserType browser)
{
    _seleniumHooks.InitializeDriver(browser);
    // Test logic
}

// ❌ OLD: Browser-specific categories
[Category("Chrome")]
public void ChromeOnlyTest()
{
    // Test logic
}
```

### After (Browser from RunSettings)
```csharp
// ✅ NEW: Browser comes from RunSettings
[TestFixture]
public class MyTests
{
    [OneTimeSetUp]
    public void Setup()
    {
        // Browser type read from RunSettings
        var browserType = Enum.Parse<BrowserType>(_config.Browser, true);
        _seleniumHooks.InitializeDriver(browserType);
    }

    [Test]
    public void MyTest()
    {
        // Same test runs in any browser
        // Browser determined by which RunSettings file is used
    }
}
```

## Creating Custom Browser Configurations

To add support for a new browser or custom configuration:

1. Create a new RunSettings file (e.g., `ChromeHeadless.runsettings`)
2. Set the browser and options:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <TestRunParameters>
    <Parameter name="Browser" value="Chrome" />
    <Parameter name="HeadlessMode" value="true" />
    <Parameter name="ScreenshotPath" value="Screenshots/ChromeHeadless" />
    <!-- Other parameters... -->
  </TestRunParameters>
</RunSettings>
```

3. Run tests with the new configuration:

```bash
dotnet test --settings RunSettings/ChromeHeadless.runsettings
```

## Best Practices

### 1. Don't Hardcode Browsers
```csharp
// ❌ Bad: Hardcoded browser
_seleniumHooks.InitializeDriver(BrowserType.Chrome);

// ✅ Good: Browser from configuration
var browserType = Enum.Parse<BrowserType>(_config.Browser, true);
_seleniumHooks.InitializeDriver(browserType);
```

### 2. Use Descriptive RunSettings Names
```
✅ Chrome.runsettings
✅ Firefox.runsettings
✅ ChromeHeadless.runsettings
✅ Dev_Chrome.runsettings
❌ Test1.runsettings
❌ Config.runsettings
```

### 3. Document Custom Configurations
If you create custom RunSettings files, document them in the RunSettings/README.md file.

### 4. Standardize in CI/CD
Use the same RunSettings files locally and in CI/CD for consistency.

## Troubleshooting

### Tests Still Using Wrong Browser

**Problem:** Tests are not using the browser specified in RunSettings.

**Solutions:**
1. Verify RunSettings file is selected in Visual Studio/VS Code
2. Check that `InitializeFromTestContext()` is called in `[OneTimeSetUp]`
3. Confirm browser is read from config: `Enum.Parse<BrowserType>(_config.Browser, true)`
4. Look for hardcoded browser values in test code

### Browser Not Found

**Problem:** "Browser not found" or driver errors.

**Solutions:**
1. Ensure WebDriver for the browser is installed
2. Check browser is installed on the machine
3. Verify browser name in RunSettings matches enum: "Chrome", "Firefox", or "Edge" (case-insensitive)

### Cross-Browser Tests Taking Too Long

**Problem:** Running tests in multiple browsers sequentially is slow.

**Solutions:**
1. Use CI/CD matrix strategy to run browsers in parallel
2. Run only smoke tests for cross-browser validation
3. Use headless mode for faster execution

## Summary

- **Browser configuration is centralized** in RunSettings files
- **Write tests once**, run in any browser by changing RunSettings
- **No browser-specific code** or attributes in tests
- **Simple cross-browser testing** via multiple test runs with different RunSettings
- **Performance optimized** with browser reuse and parallel execution

For detailed examples, see [RunSettings/README.md](RunSettings/README.md).
