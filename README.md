# Automation Test Framework

A comprehensive, enterprise-grade automation testing framework for Angular SPA applications built with .NET 8, following SOLID principles and best practices.

## Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Running Tests](#running-tests)
- [Test Reporting](#test-reporting)
- [Azure DevOps Integration](#azure-devops-integration)
- [Tag-Based Execution](#tag-based-execution)
- [Adding New Tests](#adding-new-tests)
- [Configuration](#configuration)
- [Browser Support](#browser-support)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

## Overview

This framework is designed for testing Angular Single Page Applications (SPA) with support for:
- **Multi-browser testing** (Chrome, Firefox, Edge)
- **BDD approach** using ReqNRoll (SpecFlow successor)
- **Traditional unit testing** using NUnit
- **Data-driven testing** with SQL database setup
- **Page Object Model** with Template Method pattern
- **Global browser session** (90%+ performance improvement)
- **XPath-based element location** with extensible locator framework
- **Angular-specific** page load verification
- **Comprehensive reporting** (ExtentReports, TRX, Code Coverage)
- **Azure DevOps integration** with YAML pipelines
- **Tag-based test execution** for flexible test filtering
- **CI/CD ready** with automated test reports

**📘 [See detailed architecture documentation](ARCHITECTURE.md)** for information about browser session management, page abstraction patterns, and test lifecycle.

## Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 8.0 | Runtime framework |
| C# | 12.0 | Programming language |
| ReqNRoll | 2.0.3 | BDD framework |
| NUnit | 4.1.0 | Test framework |
| Selenium WebDriver | 4.18.1 | Browser automation |
| Shouldly | 4.2.1 | Assertion library |
| ADO.NET | - | Database operations |
| Serilog | 3.1.1 | Logging framework |
| ExtentReports | 5.0.2 | HTML test reporting |

## Architecture

The framework follows a **3-layer architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────────┐
│             UITests Project                      │
│  (Test Execution, Features, Step Definitions)   │
└─────────────────┬───────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────┐
│            Common Project                        │
│  (Page Objects, Selenium Framework, Utilities)   │
└─────────────────┬───────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────┐
│          DataSetup Project                       │
│     (Database Operations, SQL Scripts)           │
└─────────────────────────────────────────────────┘
```

### SOLID Principles Implementation

1. **Single Responsibility Principle (SRP)**
   - Each class has one reason to change
   - `SqlDatabaseConnection` - only handles connections
   - `SqlScriptExecutor` - only executes scripts
   - `WebDriverFactory` - only creates drivers

2. **Open/Closed Principle (OCP)**
   - Framework is open for extension, closed for modification
   - New browser types can be added to `WebDriverFactory`
   - New locator strategies can be added to `ElementLocatorFactory`

3. **Liskov Substitution Principle (LSP)**
   - All page objects can substitute `BasePage`
   - All implementations follow their interface contracts

4. **Interface Segregation Principle (ISP)**
   - Focused interfaces: `IDatabaseConnection`, `IElementLocator`, `IElementActions`
   - Clients depend only on interfaces they use

5. **Dependency Inversion Principle (DIP)**
   - High-level modules depend on abstractions
   - `SqlScriptExecutor` depends on `IDatabaseConnection` interface

## Project Structure

```
AutomationTest/
│
├── DataSetup/                          # Database setup and management
│   ├── Interfaces/
│   │   ├── IDatabaseConnection.cs      # Database connection interface
│   │   └── ISqlScriptExecutor.cs       # Script executor interface
│   ├── Implementations/
│   │   ├── SqlDatabaseConnection.cs    # SQL Server connection
│   │   └── SqlScriptExecutor.cs        # SQL script execution
│   ├── SqlScripts/
│   │   ├── SampleData/                 # Data setup scripts
│   │   │   └── CreateTestUsers.sql
│   │   └── Cleanup/                    # Cleanup scripts
│   │       └── CleanupTestUsers.sql
│   └── DataSetupManager.cs             # Facade for data operations
│
├── Common/                             # Shared utilities and framework
│   ├── Configuration/
│   │   └── TestConfiguration.cs        # Configuration manager
│   ├── Enums/
│   │   ├── BrowserType.cs             # Browser enumeration
│   │   └── LocatorType.cs             # Locator strategy enum
│   ├── Selenium/
│   │   ├── Interfaces/
│   │   │   ├── IWebDriverFactory.cs   # Driver factory interface
│   │   │   ├── IElementLocator.cs     # Element locator interface
│   │   │   └── IElementActions.cs     # Element actions interface
│   │   ├── Implementations/
│   │   │   ├── WebDriverFactory.cs    # Browser driver creation
│   │   │   ├── XPathElementLocator.cs # XPath locator
│   │   │   └── ElementLocatorFactory.cs # Locator factory
│   │   └── SeleniumHooks.cs           # Central Selenium operations
│   ├── PageObjects/
│   │   ├── AbstractPage.cs            # Abstract page template
│   │   └── BasePage.cs                # Base page with common methods
│   ├── Utilities/
│   │   └── Logger.cs                  # Logging utility
│   ├── Extensions/
│   │   ├── WebDriverExtensions.cs     # WebDriver extensions
│   │   └── WebElementExtensions.cs    # WebElement extensions
│   └── appsettings.json               # Configuration file
│
└── UITests/                            # Test project
    ├── Features/
    │   └── AngularHomePage.feature    # BDD feature files
    ├── StepDefinitions/
    │   └── AngularHomePageSteps.cs    # Step definitions
    ├── Tests/
    │   └── AngularHomePageNUnitTests.cs # NUnit tests
    ├── Pages/
    │   └── AngularHomePage.cs         # Page object
    ├── Hooks/
    │   └── TestHooks.cs               # ReqNRoll hooks
    └── appsettings.json               # Test configuration
```

## Getting Started

### Prerequisites

1. **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Visual Studio 2022** or **Visual Studio Code**
3. **Browser Drivers** (automatically managed by Selenium Manager)
4. **SQL Server** (optional, for data setup features)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd AutomationTest
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Update configuration**
   - Edit `Common/appsettings.json` and `UITests/appsettings.json`
   - Set your `BaseUrl`, `Browser`, and other preferences

### Configuration

Edit `appsettings.json` to configure test settings:

```json
{
  "TestSettings": {
    "BaseUrl": "https://angular.io",
    "Browser": "Chrome",
    "ImplicitWaitSeconds": 10,
    "ExplicitWaitSeconds": 30,
    "PageLoadTimeoutSeconds": 60,
    "HeadlessMode": false,
    "ScreenshotOnFailure": true,
    "ScreenshotPath": "Screenshots"
  },
  "DatabaseSettings": {
    "ConnectionString": "Server=localhost;Database=TestDB;Integrated Security=true;TrustServerCertificate=True;"
  }
}
```

## Running Tests

### Using Visual Studio Test Explorer

1. Open the solution in Visual Studio
2. Open **Test Explorer** (Test > Test Explorer)
3. Click **Run All** or select specific tests

### Using Command Line

**Run all tests:**
```bash
dotnet test
```

**Run specific test category:**
```bash
dotnet test --filter TestCategory=Smoke
```

**Run tests in specific project:**
```bash
dotnet test UITests/UITests.csproj
```

**Run ReqNRoll tests with specific tag:**
```bash
dotnet test --filter Category=chrome
```

### Running with Different Browsers

**Option 1: Using ReqNRoll tags**
```gherkin
@chrome
Scenario: Test in Chrome

@firefox
Scenario: Test in Firefox

@edge
Scenario: Test in Edge
```

**Option 2: Using NUnit TestCase**
```csharp
[TestCase(BrowserType.Chrome)]
[TestCase(BrowserType.Firefox)]
[TestCase(BrowserType.Edge)]
public void TestInMultipleBrowsers(BrowserType browser)
{
    // Test implementation
}
```

**Option 3: Update appsettings.json**
```json
{
  "TestSettings": {
    "Browser": "Firefox"
  }
}
```

## Adding New Tests

### Adding a BDD Test (ReqNRoll)

**Step 1: Create a Feature File**

Create `UITests/Features/NewFeature.feature`:
```gherkin
Feature: New Feature
    As a user
    I want to test new functionality
    So that I can ensure it works correctly

@chrome
Scenario: Test scenario name
    Given I navigate to the page
    When I perform an action
    Then I should see expected result
```

**Step 2: Create Step Definitions**

Create `UITests/StepDefinitions/NewFeatureSteps.cs`:
```csharp
using Reqnroll;

[Binding]
public class NewFeatureSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly SeleniumHooks _seleniumHooks;

    public NewFeatureSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _seleniumHooks = (SeleniumHooks)_scenarioContext["SeleniumHooks"];
    }

    [Given(@"I navigate to the page")]
    public void GivenINavigateToThePage()
    {
        // Implementation
    }
}
```

### Adding a NUnit Test

Create `UITests/Tests/NewTest.cs`:
```csharp
using NUnit.Framework;

[TestFixture]
public class NewTest
{
    private SeleniumHooks _seleniumHooks;

    [SetUp]
    public void Setup()
    {
        _seleniumHooks = new SeleniumHooks();
        _seleniumHooks.InitializeDriver(BrowserType.Chrome);
    }

    [TearDown]
    public void TearDown()
    {
        _seleniumHooks?.QuitDriver();
    }

    [Test]
    public void TestMethod()
    {
        // Test implementation
    }
}
```

### Adding a New Page Object

**Step 1: Create Page Class**

Create `UITests/Pages/NewPage.cs`:
```csharp
using Common.PageObjects;
using Common.Selenium;

public class NewPage : BasePage
{
    // XPath locators
    private const string ElementXPath = "//div[@id='element']";

    public NewPage(SeleniumHooks seleniumHooks) : base(seleniumHooks)
    {
    }

    public override bool IsPageLoaded()
    {
        WaitForPageLoad();
        WaitForAngularLoad();
        return IsElementDisplayed(ElementXPath);
    }

    public void ClickElement()
    {
        ClickElement(ElementXPath);
    }
}
```

## Browser Support

### Supported Browsers

| Browser | Minimum Version | Driver Management |
|---------|----------------|-------------------|
| Chrome | 90+ | Selenium Manager |
| Firefox | 90+ | Selenium Manager |
| Edge | 90+ | Selenium Manager |

### Browser Configuration

Browsers are configured in `WebDriverFactory.cs`. To add support for a new browser:

1. Add browser type to `BrowserType` enum
2. Create driver method in `WebDriverFactory`
3. Add case to switch statement in `CreateDriver`

## Best Practices

### Page Object Guidelines

1. **Use XPath locators** - All element locators should use XPath as primary strategy
2. **Encapsulate locators** - Store locators as private constants
3. **One action per method** - Each method should perform one logical action
4. **Return meaningful types** - Return page objects for navigation, bool for assertions
5. **Use descriptive names** - Method names should clearly describe their purpose

### Test Writing Guidelines

1. **Follow AAA pattern** - Arrange, Act, Assert
2. **Use meaningful assertions** - Use Shouldly for readable assertions
3. **Tag appropriately** - Use tags for categorization (@smoke, @regression, @chrome)
4. **Keep tests independent** - Each test should be able to run standalone
5. **Use data-driven tests** - Parameterize tests for different data sets

### Element Location Best Practices

```csharp
// Good: Specific, maintainable XPath
private const string LoginButton = "//button[@id='login' and @type='submit']";

// Bad: Generic, brittle XPath
private const string LoginButton = "//button[1]";

// Good: Using locator with wait
WaitForElement(LoginButton);
ClickElement(LoginButton);

// Bad: Direct click without wait
ClickElement(LoginButton);
```

## Troubleshooting

### Common Issues

**Issue: "WebDriver not initialized"**
- **Solution**: Ensure `InitializeDriver()` is called in `[SetUp]` or `[BeforeScenario]`

**Issue: "Element not found"**
- **Solution**: Verify XPath is correct, add explicit waits
- Use browser DevTools to test XPath: `$x("your-xpath")`

**Issue: "Angular page not loading"**
- **Solution**: Increase timeout in `WaitForAngular()` method
- Verify Angular is present on page

**Issue: "Browser driver not found"**
- **Solution**: Selenium Manager handles drivers automatically in v4.6+
- Manually download drivers if needed

### Debug Mode

Enable detailed logging:
```json
{
  "Logging": {
    "LogLevel": "Debug"
  }
}
```

### Screenshot on Failure

Screenshots are automatically captured on test failure when configured:
```json
{
  "TestSettings": {
    "ScreenshotOnFailure": true,
    "ScreenshotPath": "Screenshots"
  }
}
```

## Maintenance Guidelines

### Regular Maintenance Tasks

1. **Update NuGet packages** regularly
   ```bash
   dotnet list package --outdated
   dotnet add package <PackageName>
   ```

2. **Review and refactor** page objects monthly
3. **Update XPath locators** when UI changes
4. **Archive old screenshots** and logs
5. **Review and update** test data scripts

### Adding New Features

1. Create feature branch
2. Add tests following existing patterns
3. Update README if adding new capabilities
4. Submit pull request with test results

## Project-Specific README Files

For detailed information about each project, see:
- [DataSetup Project README](DataSetup/README.md)
- [Common Project README](Common/README.md)
- [UITests Project README](UITests/README.md)

## Support and Contribution

For issues, questions, or contributions:
1. Check existing documentation
2. Review troubleshooting section
3. Create an issue with detailed description
4. Follow the contribution guidelines

---

**Framework Version:** 1.0.0
**Last Updated:** 2025-01-04
**Maintainer:** Automation Team
