# Performance Optimization Guide

## Overview

This document describes the performance optimizations implemented in the test automation framework to maximize test execution efficiency and minimize browser overhead.

## Key Performance Improvements

### 1. Browser Instance Reuse (Global Session)

**Before Optimization:**
- Browser opened and closed for EVERY single test
- 10 tests = 10 browser launches (very slow)
- Example: 5 seconds per browser launch × 10 tests = 50 seconds overhead

**After Optimization (Current Architecture):**
- Browser opened ONCE for the ENTIRE test run (not per fixture, but globally!)
- 100 tests across all fixtures = 1 browser launch
- Example: 5 seconds per browser launch × 1 = 5 seconds overhead
- **Result: 90%+ reduction in browser overhead**

**Implementation:**
- Uses `BrowserSessionManager` singleton pattern
- Browser initialized in `GlobalSetup` ([SetUpFixture])
- All test fixtures share the same browser instance
- Session clearing between tests maintains isolation

#### Implementation Details

**Global Browser Setup (Runs ONCE for entire test run):**

```csharp
// UITests/GlobalSetup.cs
[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void GlobalOneTimeSetup()
    {
        // Initialize browser ONCE for ALL tests in the assembly
        var sessionManager = BrowserSessionManager.Instance;
        var browserType = Enum.Parse<BrowserType>(config.Browser, true);
        sessionManager.InitializeBrowser(browserType);
    }

    [OneTimeTearDown]
    public void GlobalOneTimeTearDown()
    {
        // Quit browser ONCE after ALL tests complete
        BrowserSessionManager.Instance.QuitBrowser();
    }
}
```

**Test Fixture Usage (Reuses global browser):**

```csharp
[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class AngularHomePageLoginTests
{
    private BrowserSessionManager? _sessionManager;
    private SeleniumHooks? _seleniumHooks;
    private AngularHomePage? _angularHomePage;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        // Get the SHARED browser session (already initialized)
        _sessionManager = BrowserSessionManager.Instance;
        _seleniumHooks = _sessionManager.GetBrowserSession();
        _angularHomePage = new AngularHomePage(_seleniumHooks);
    }

    [SetUp]
    public void Setup()
    {
        // Just navigate to page (fast)
        _angularHomePage!.NavigateToHomePage(_config!.BaseUrl);
    }

    [TearDown]
    public void TearDown()
    {
        // Clear session for test isolation (faster than restarting browser)
        _sessionManager?.ClearSession(); // Clears cookies, localStorage, sessionStorage
    }
}
```

### 2. Parallel Test Execution

**Configuration in RunSettings:**

```xml
<RunConfiguration>
  <!-- 0 = use all available CPU cores -->
  <MaxCpuCount>0</MaxCpuCount>
</RunConfiguration>

<NUnit>
  <!-- Number of parallel test workers -->
  <NumberOfTestWorkers>4</NumberOfTestWorkers>
</NUnit>
```

**Environment-Specific Workers:**
- **Dev**: 4 workers (for developer machines)
- **QA**: 6 workers (for build servers)
- **Prod**: 8 workers (for high-performance CI/CD)

**Parallelization Attributes:**

```csharp
// Allow fixture to run in parallel with other fixtures
[Parallelizable(ParallelScope.Fixtures)]
public class AngularHomePageCrossBrowserTests
{
    // Tests within fixture run sequentially but share browser
}

// Allow tests within fixture to run in parallel
[Parallelizable(ParallelScope.Self)]
public class IndependentTests
{
    // Each test runs independently
}
```

### 3. Cross-Browser Testing Optimization

**Before Optimization:**
```csharp
[TestCase(BrowserType.Chrome)]
[TestCase(BrowserType.Firefox)]
[TestCase(BrowserType.Edge)]
public void TestInBrowser(BrowserType browser)
{
    // Browser switched for each test case
    // 3 browsers × N tests = 3N browser launches
}
```

**After Optimization:**
```csharp
// Each browser runs in separate fixture - can run in parallel!
[TestFixture(BrowserType.Chrome)]
[TestFixture(BrowserType.Firefox)]
[TestFixture(BrowserType.Edge)]
[Parallelizable(ParallelScope.Fixtures)]
public class CrossBrowserTests
{
    private readonly BrowserType _browserType;

    public CrossBrowserTests(BrowserType browserType)
    {
        _browserType = browserType;
    }

    [OneTimeSetUp]
    public void Setup()
    {
        // Each fixture opens its browser ONCE
        _seleniumHooks.InitializeDriver(_browserType);
    }
}
```

**Benefits:**
- Chrome, Firefox, and Edge fixtures run in parallel
- Each browser instance reused across all tests in that fixture
- 3× faster cross-browser testing

### 4. Efficient Page Navigation

**Before:**
```csharp
[Test]
public void Test1()
{
    NavigateToHomePage(); // Full page load
    // Assert
}

[Test]
public void Test2()
{
    NavigateToHomePage(); // Full page load again
    // Assert
}
```

**After:**
```csharp
[SetUp]
public void Setup()
{
    // Navigate once before each test
    _angularHomePage.NavigateToHomePage(_config.BaseUrl);
}

[Test]
public void Test1()
{
    // Page already loaded - just assert
}

[Test]
public void Test2()
{
    // Page already loaded - just assert
}
```

### 5. Test Isolation Without Browser Restart

**Cookie Clearing for Test Isolation:**

```csharp
[TearDown]
public void TearDown()
{
    // Clear cookies/session between tests (milliseconds)
    // Instead of browser restart (seconds)
    _seleniumHooks?.GetDriver().Manage().Cookies.DeleteAllCookies();
}
```

This maintains test isolation while avoiding expensive browser restarts.

## Performance Metrics

### Expected Performance Gains

| Scenario | Before | After | Improvement |
|----------|--------|-------|-------------|
| 10 tests in 1 fixture | ~80 sec | ~15 sec | **81% faster** |
| Cross-browser (3 browsers, 5 tests each) | ~120 sec | ~25 sec | **79% faster** |
| 50 tests with parallel execution | ~400 sec | ~60 sec | **85% faster** |

### Real-World Example

**Scenario: 20 login tests across 3 browsers (60 total tests)**

**Before Optimization:**
- 60 browser launches × 5 seconds = 300 seconds browser overhead
- Sequential execution = 300 + (60 tests × 3 sec) = 480 seconds total
- **Total: 8 minutes**

**After Optimization:**
- 3 browser launches × 5 seconds = 15 seconds browser overhead
- Parallel execution (3 fixtures × 20 tests / 4 workers) = 15 + (20 tests × 3 sec / 4) = 30 seconds
- **Total: 30 seconds**

**Result: 16× faster! (480s → 30s)**

## Best Practices

### 1. Group Related Tests

```csharp
// Good: All login tests share one browser instance
[TestFixture]
public class LoginTests
{
    // 10 tests here = 1 browser launch
}

// Good: All profile tests share another browser instance
[TestFixture]
public class ProfileTests
{
    // 10 tests here = 1 browser launch
}
```

### 2. Use Static Fields for Shared Resources

```csharp
private static SeleniumHooks? _seleniumHooks;  // Static!
private static AngularHomePage? _angularHomePage;  // Static!
```

Static fields ensure the same instance is shared across all tests in the fixture.

### 3. Clear State Between Tests

```csharp
[TearDown]
public void TearDown()
{
    // Clear cookies
    _driver.Manage().Cookies.DeleteAllCookies();

    // Clear local storage if needed
    ((IJavaScriptExecutor)_driver).ExecuteScript("localStorage.clear();");

    // Clear session storage if needed
    ((IJavaScriptExecutor)_driver).ExecuteScript("sessionStorage.clear();");
}
```

### 4. Choose Appropriate Parallelization

```csharp
// For independent test fixtures (can run simultaneously)
[Parallelizable(ParallelScope.Fixtures)]

// For tests that can run in parallel within fixture
[Parallelizable(ParallelScope.Self)]

// For tests that must run sequentially
[Parallelizable(ParallelScope.None)]
```

### 5. Monitor Resource Usage

```bash
# Run with specific number of workers
dotnet test --settings RunSettings/Dev.runsettings

# Monitor during execution:
# - CPU usage (should be high with parallel execution)
# - Memory usage (each browser uses ~200-500MB)
# - Adjust NumberOfTestWorkers if needed
```

## Files Modified for Performance

1. **[UITests/Tests/AngularHomePageLoginTests.cs](UITests/Tests/AngularHomePageLoginTests.cs)**
   - Changed to `static` fields for browser reuse
   - Added `[OneTimeSetUp]` and `[OneTimeTearDown]`
   - Added `[Parallelizable]` attributes
   - Removed redundant navigation

2. **[UITests/Tests/AngularHomePageNUnitTests.cs](UITests/Tests/AngularHomePageNUnitTests.cs)**
   - Same optimizations as above
   - Cross-browser tests use parameterized fixtures

3. **[RunSettings/Dev.runsettings](RunSettings/Dev.runsettings)**
   - `MaxCpuCount` set to `0` (use all cores)
   - `NumberOfTestWorkers` set to `4`

4. **[RunSettings/QA.runsettings](RunSettings/QA.runsettings)**
   - `NumberOfTestWorkers` set to `6`

5. **[RunSettings/Prod.runsettings](RunSettings/Prod.runsettings)**
   - `NumberOfTestWorkers` set to `8`
   - `HeadlessMode` enabled for maximum performance

## Running Optimized Tests

### Sequential Execution (for debugging)

Create a `Sequential.runsettings`:
```xml
<RunConfiguration>
  <MaxCpuCount>1</MaxCpuCount>
</RunConfiguration>
<NUnit>
  <NumberOfTestWorkers>1</NumberOfTestWorkers>
</NUnit>
```

### Parallel Execution (normal)

```bash
# Use Dev/QA/Prod settings (already configured for parallel)
dotnet test --settings RunSettings/Dev.runsettings
```

### Monitor Execution

```bash
# Verbose output to see parallel execution
dotnet test --settings RunSettings/Dev.runsettings --logger "console;verbosity=detailed"
```

## Troubleshooting

### Tests Fail in Parallel but Pass Sequentially

**Problem:** Tests are not properly isolated

**Solutions:**
1. Ensure cookies are cleared in `[TearDown]`
2. Check for shared state between tests
3. Use `[Parallelizable(ParallelScope.None)]` for problematic fixtures

### High Memory Usage

**Problem:** Too many parallel browsers

**Solution:**
Reduce `NumberOfTestWorkers`:
```xml
<NUnit>
  <NumberOfTestWorkers>2</NumberOfTestWorkers>
</NUnit>
```

### Browser Instances Not Closing

**Problem:** `[OneTimeTearDown]` not called

**Solutions:**
1. Ensure test run completes (not cancelled)
2. Add try-finally in tests
3. Check for exceptions in `[OneTimeSetUp]`

## Additional Optimizations

### 1. Use Headless Mode in CI/CD

```xml
<Parameter name="HeadlessMode" value="true" />
```

Headless browsers are ~30% faster and use ~50% less memory.

### 2. Reduce Wait Times

```xml
<Parameter name="ImplicitWaitSeconds" value="5" />
<Parameter name="ExplicitWaitSeconds" value="15" />
```

Tune wait times based on your application's performance.

### 3. Disable Screenshots for Passing Tests

Screenshots only on failure saves time:
```csharp
if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
{
    TakeScreenshot();
}
```

### 4. Use Efficient Locators

- XPath is already optimized in this framework
- Avoid complex XPath with multiple predicates
- Cache commonly used elements

## Conclusion

These optimizations provide:
- **80-90% reduction** in browser overhead
- **3-10× faster** test execution with parallelization
- **Better resource utilization** on CI/CD servers
- **Maintained test isolation** without performance penalty

The framework now efficiently reuses browser instances while maintaining clean test separation through cookie clearing and page navigation.
