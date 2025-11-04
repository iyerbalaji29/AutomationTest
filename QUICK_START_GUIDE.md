# Quick Start Guide

Get up and running with the Automation Test Framework in 5 minutes!

## Prerequisites

Before you begin, ensure you have:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed
- Visual Studio 2022 or Visual Studio Code
- Git (optional, for version control)

## Step 1: Get the Code

```bash
cd F:\repos\AutomationTest
```

## Step 2: Restore Dependencies

Open a terminal in the project directory and run:

```bash
dotnet restore
```

This will download all required NuGet packages.

## Step 3: Build the Solution

```bash
dotnet build
```

You should see a successful build message for all three projects:
- DataSetup
- Common
- UITests

## Step 4: Configure Settings (Optional)

The framework comes with default settings that work out of the box. If you want to customize:

Edit `UITests/appsettings.json`:

```json
{
  "TestSettings": {
    "BaseUrl": "https://angular.io",
    "Browser": "Chrome",
    "HeadlessMode": false
  }
}
```

**Common Settings to Change:**
- `BaseUrl`: Your application URL
- `Browser`: Chrome, Firefox, or Edge
- `HeadlessMode`: true for headless execution

## Step 5: Run Your First Test

### Option A: Using Command Line

```bash
dotnet test
```

This runs all tests in the solution.

### Option B: Using Visual Studio

1. Open `AutomationTest.sln` in Visual Studio
2. Go to **Test** → **Test Explorer**
3. Click **Run All**

### Option C: Run Specific Tests

```bash
# Run only smoke tests
dotnet test --filter "Category=smoke"

# Run only Chrome tests
dotnet test --filter "Category=chrome"

# Run NUnit tests only
dotnet test UITests/Tests/
```

## Step 6: View Results

### Test Output
Test results appear in the console:
```
Passed!  - Failed:     0, Passed:     5, Skipped:     0, Total:     5
```

### Screenshots
Failed test screenshots are saved to: `Screenshots/`

### Logs
Detailed logs are saved to: `Logs/TestLog_YYYYMMDD.txt`

## Next Steps

### Add Your First Test

**1. Create a Feature File** (BDD approach)

Create `UITests/Features/MyFirstTest.feature`:

```gherkin
Feature: My First Test
    As a new user
    I want to test the framework
    So that I can learn how it works

@chrome @smoke
Scenario: Verify page loads
    Given I navigate to the Angular home page
    Then the Angular page should be loaded successfully
```

**2. Run Your Test**

```bash
dotnet test --filter "Category=smoke"
```

### Create Your First Page Object

Create `UITests/Pages/MyPage.cs`:

```csharp
using Common.PageObjects;
using Common.Selenium;
using Shouldly;

namespace UITests.Pages
{
    public class MyPage : BasePage
    {
        private const string PageTitleXPath = "//h1";

        public MyPage(SeleniumHooks seleniumHooks) : base(seleniumHooks)
        {
        }

        public override bool IsPageLoaded()
        {
            WaitForPageLoad();
            WaitForAngularLoad();
            return IsElementDisplayed(PageTitleXPath);
        }

        public void NavigateToMyPage()
        {
            NavigateTo("https://example.com");
        }

        public void AssertPageTitleContains(string expectedText)
        {
            var title = GetPageTitle();
            title.ShouldContain(expectedText);
        }
    }
}
```

## Common Commands Cheat Sheet

```bash
# Build solution
dotnet build

# Run all tests
dotnet test

# Run tests with specific tag
dotnet test --filter "Category=smoke"

# Run tests in specific project
dotnet test UITests/UITests.csproj

# Run with verbose output
dotnet test -v detailed

# Run in headless mode
# (Update appsettings.json: "HeadlessMode": true)
dotnet test

# Clean build artifacts
dotnet clean

# Restore packages
dotnet restore
```

## Browser Support

The framework automatically manages browser drivers for:
- **Chrome** (default)
- **Firefox**
- **Edge**

To run tests in different browsers:

```bash
# Update appsettings.json
"Browser": "Firefox"

# Or use tags in feature files
@firefox
Scenario: Test in Firefox
```

## Troubleshooting Quick Fixes

**Issue: "Build failed"**
```bash
dotnet clean
dotnet restore
dotnet build
```

**Issue: "Tests not discovered"**
- Rebuild the solution
- Close and reopen Test Explorer

**Issue: "WebDriver not found"**
- Selenium 4.6+ manages drivers automatically
- Ensure you have internet connection on first run

**Issue: "Element not found"**
- Check XPath with browser DevTools: Press F12, go to Console, type:
  ```javascript
  $x("//your/xpath/here")
  ```

## Project Structure Overview

```
AutomationTest/
├── DataSetup/          # Database setup (SQL scripts)
├── Common/             # Framework code (Selenium, Page Objects)
├── UITests/            # Your tests (Features, Tests, Pages)
│   ├── Features/       # BDD scenarios (.feature files)
│   ├── Tests/          # NUnit tests
│   ├── Pages/          # Page Objects
│   └── StepDefinitions/# BDD step implementations
├── Screenshots/        # Test failure screenshots
└── Logs/              # Test execution logs
```

## Learning Path

1. **Read the README** - [README.md](README.md)
2. **Explore Sample Tests** - `UITests/Features/AngularHomePage.feature`
3. **Study Page Objects** - `UITests/Pages/AngularHomePage.cs`
4. **Review Step Definitions** - `UITests/StepDefinitions/AngularHomePageSteps.cs`
5. **Read Project READMEs**:
   - [DataSetup README](DataSetup/README.md)
   - [Common README](Common/README.md)
   - [UITests README](UITests/README.md)

## Getting Help

- **Check documentation** in README files
- **Review example tests** in the UITests project
- **Enable debug logging** in appsettings.json
- **Take screenshots** on failures (enabled by default)

## What's Next?

Now that you're up and running:

1. **Customize for your application**
   - Update `BaseUrl` in appsettings.json
   - Create page objects for your pages
   - Write tests for your scenarios

2. **Learn the framework patterns**
   - Study SOLID principles implementation
   - Understand the Page Object Model
   - Master XPath element location

3. **Add CI/CD integration**
   - Run tests in your build pipeline
   - Generate test reports
   - Set up scheduled test runs

Happy Testing!
