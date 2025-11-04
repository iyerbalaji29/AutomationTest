# UITests Project

This project contains the actual test implementations using both BDD (ReqNRoll) and traditional NUnit approaches for testing Angular SPA applications.

## Purpose

The UITests project provides:
- **BDD test scenarios** using ReqNRoll (Gherkin syntax)
- **Traditional unit tests** using NUnit
- **Page Object implementations** for specific pages
- **Step definitions** for BDD scenarios
- **Test hooks** for setup and teardown
- **Multi-browser test execution**

## Architecture

```
UITests/
├── Features/               # BDD feature files (.feature)
├── StepDefinitions/        # ReqNRoll step implementations
├── Tests/                  # Traditional NUnit tests
├── Pages/                  # Page Object implementations
├── Hooks/                  # ReqNRoll hooks (setup/teardown)
└── appsettings.json        # Test configuration
```

## Project Structure

### Features/
Contains Gherkin feature files describing test scenarios in business language.

**Example: AngularHomePage.feature**
```gherkin
Feature: Angular Home Page Tests
    As a tester
    I want to validate the Angular home page
    So that I can ensure all UI elements are working correctly

Background:
    Given I navigate to the Angular home page

@chrome @smoke
Scenario: Verify Angular home page loads successfully
    Then the Angular page should be loaded successfully
    And the Angular logo should be displayed
    And the page title should contain "Angular"
```

### StepDefinitions/
Contains C# implementations of Gherkin steps.

**Example: AngularHomePageSteps.cs**
```csharp
[Binding]
public class AngularHomePageSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly SeleniumHooks _seleniumHooks;
    private readonly AngularHomePage _angularHomePage;

    public AngularHomePageSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _seleniumHooks = (SeleniumHooks)_scenarioContext["SeleniumHooks"];
        _angularHomePage = new AngularHomePage(_seleniumHooks);
    }

    [Given(@"I navigate to the Angular home page")]
    public void GivenINavigateToTheAngularHomePage()
    {
        _angularHomePage.NavigateToHomePage(_config.BaseUrl);
    }

    [Then(@"the Angular logo should be displayed")]
    public void ThenTheAngularLogoShouldBeDisplayed()
    {
        _angularHomePage.AssertLogoIsDisplayed();
    }
}
```

### Tests/
Contains traditional NUnit test classes.

**Example: AngularHomePageNUnitTests.cs**
```csharp
[TestFixture]
[Category("Smoke")]
public class AngularHomePageNUnitTests
{
    private SeleniumHooks _seleniumHooks;
    private AngularHomePage _angularHomePage;

    [SetUp]
    public void Setup()
    {
        _seleniumHooks = new SeleniumHooks();
        _seleniumHooks.InitializeDriver(BrowserType.Chrome);
        _angularHomePage = new AngularHomePage(_seleniumHooks);
    }

    [TearDown]
    public void TearDown()
    {
        _seleniumHooks?.QuitDriver();
    }

    [Test]
    public void VerifyAngularHomePageLoads()
    {
        _angularHomePage.NavigateToHomePage(_config.BaseUrl);
        _angularHomePage.AssertAngularPageLoaded();
    }
}
```

### Pages/
Contains page-specific implementations inheriting from BasePage.

**Example: AngularHomePage.cs**
```csharp
public class AngularHomePage : BasePage
{
    private const string HomePageLogoXPath = "//a[@aria-label='Angular']";
    private const string GetStartedButtonXPath = "//a[contains(text(),'Get Started')]";

    public AngularHomePage(SeleniumHooks seleniumHooks) : base(seleniumHooks)
    {
    }

    public override bool IsPageLoaded()
    {
        WaitForPageLoad();
        WaitForAngularLoad();
        return IsElementDisplayed(HomePageLogoXPath);
    }

    public void AssertLogoIsDisplayed()
    {
        IsElementDisplayed(HomePageLogoXPath)
            .ShouldBeTrue("Angular logo should be displayed");
    }
}
```

### Hooks/
Contains ReqNRoll hooks for test lifecycle management.

**TestHooks.cs** - Manages WebDriver initialization and cleanup.

## Adding New Tests

### Option 1: BDD Approach (ReqNRoll)

#### Step 1: Create Feature File

Create `Features/NewFeature.feature`:

```gherkin
Feature: New Feature Name
    As a user
    I want to perform some action
    So that I can achieve some goal

Background:
    Given I am on the application homepage

@chrome @smoke
Scenario: First test scenario
    Given I have navigated to the target page
    When I click on the submit button
    Then I should see a success message

@firefox @regression
Scenario: Second test scenario
    Given I have entered valid data
    When I submit the form
    Then the data should be saved successfully
```

#### Step 2: Create Step Definitions

Create `StepDefinitions/NewFeatureSteps.cs`:

```csharp
using Common.Selenium;
using Reqnroll;
using UITests.Pages;

namespace UITests.StepDefinitions
{
    [Binding]
    public class NewFeatureSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly SeleniumHooks _seleniumHooks;
        private readonly NewFeaturePage _newFeaturePage;

        public NewFeatureSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _seleniumHooks = (SeleniumHooks)_scenarioContext["SeleniumHooks"];
            _newFeaturePage = new NewFeaturePage(_seleniumHooks);
        }

        [Given(@"I have navigated to the target page")]
        public void GivenIHaveNavigatedToTheTargetPage()
        {
            _newFeaturePage.NavigateToPage();
        }

        [When(@"I click on the submit button")]
        public void WhenIClickOnTheSubmitButton()
        {
            _newFeaturePage.ClickSubmitButton();
        }

        [Then(@"I should see a success message")]
        public void ThenIShouldSeeASuccessMessage()
        {
            _newFeaturePage.AssertSuccessMessageDisplayed();
        }
    }
}
```

#### Step 3: Create Page Object

Create `Pages/NewFeaturePage.cs`:

```csharp
using Common.PageObjects;
using Common.Selenium;
using Shouldly;

namespace UITests.Pages
{
    public class NewFeaturePage : BasePage
    {
        // XPath locators
        private const string SubmitButtonXPath = "//button[@id='submit']";
        private const string SuccessMessageXPath = "//div[@class='success-message']";

        public NewFeaturePage(SeleniumHooks seleniumHooks) : base(seleniumHooks)
        {
        }

        public override bool IsPageLoaded()
        {
            WaitForPageLoad();
            WaitForAngularLoad();
            return IsElementDisplayed(SubmitButtonXPath);
        }

        public void NavigateToPage()
        {
            NavigateTo("/new-feature");
            WaitForAngularLoad();
        }

        public void ClickSubmitButton()
        {
            WaitForElement(SubmitButtonXPath);
            ClickElement(SubmitButtonXPath);
        }

        public void AssertSuccessMessageDisplayed()
        {
            WaitForElement(SuccessMessageXPath, 30);
            IsElementDisplayed(SuccessMessageXPath)
                .ShouldBeTrue("Success message should be displayed");
        }
    }
}
```

#### Step 4: Run the Tests

```bash
# Run all scenarios
dotnet test

# Run specific tag
dotnet test --filter "Category=smoke"

# Run specific browser
dotnet test --filter "Category=chrome"
```

### Option 2: NUnit Approach

#### Step 1: Create Test Class

Create `Tests/NewFeatureTests.cs`:

```csharp
using Common.Configuration;
using Common.Enums;
using Common.Selenium;
using NUnit.Framework;
using UITests.Pages;

namespace UITests.Tests
{
    [TestFixture]
    [Category("NewFeature")]
    public class NewFeatureTests
    {
        private SeleniumHooks _seleniumHooks;
        private NewFeaturePage _newFeaturePage;
        private TestConfiguration _config;

        [SetUp]
        public void Setup()
        {
            _config = TestConfiguration.Instance;
            _seleniumHooks = new SeleniumHooks();
            _seleniumHooks.InitializeDriver(BrowserType.Chrome);
            _newFeaturePage = new NewFeaturePage(_seleniumHooks);
        }

        [TearDown]
        public void TearDown()
        {
            _seleniumHooks?.QuitDriver();
        }

        [Test]
        [Category("Smoke")]
        [Description("Verify form submission works correctly")]
        public void VerifyFormSubmission()
        {
            // Arrange
            _newFeaturePage.NavigateToPage();

            // Act
            _newFeaturePage.ClickSubmitButton();

            // Assert
            _newFeaturePage.AssertSuccessMessageDisplayed();
        }

        [Test]
        [TestCase("valid@email.com", "ValidUser")]
        [TestCase("another@email.com", "AnotherUser")]
        [Description("Data-driven test with multiple inputs")]
        public void VerifyMultipleInputs(string email, string username)
        {
            // Arrange
            _newFeaturePage.NavigateToPage();

            // Act
            _newFeaturePage.EnterEmail(email);
            _newFeaturePage.EnterUsername(username);
            _newFeaturePage.ClickSubmitButton();

            // Assert
            _newFeaturePage.AssertSuccessMessageDisplayed();
        }
    }
}
```

#### Step 2: Run the Tests

```bash
# Run all tests in the class
dotnet test --filter ClassName=NewFeatureTests

# Run specific test
dotnet test --filter Name=VerifyFormSubmission

# Run by category
dotnet test --filter TestCategory=Smoke
```

## Multi-Browser Testing

### Using ReqNRoll Tags

Add browser tags to scenarios:

```gherkin
@chrome
Scenario: Test in Chrome
    Given I navigate to the page
    Then the page should load

@firefox
Scenario: Test in Firefox
    Given I navigate to the page
    Then the page should load

@edge
Scenario: Test in Edge
    Given I navigate to the page
    Then the page should load
```

The `TestHooks.cs` automatically detects browser tags and initializes the appropriate driver.

### Using NUnit TestCase

Create parameterized tests:

```csharp
[TestFixture]
public class CrossBrowserTests
{
    [Test]
    [TestCase(BrowserType.Chrome)]
    [TestCase(BrowserType.Firefox)]
    [TestCase(BrowserType.Edge)]
    public void TestInMultipleBrowsers(BrowserType browser)
    {
        var hooks = new SeleniumHooks();
        hooks.InitializeDriver(browser);

        try
        {
            // Test implementation
        }
        finally
        {
            hooks.QuitDriver();
        }
    }
}
```

## Test Organization Best Practices

### 1. Feature File Organization

```gherkin
Feature: Clear, descriptive feature name
    As a [role]
    I want to [perform action]
    So that I can [achieve goal]

Background:
    Given common setup for all scenarios

@tag1 @tag2
Scenario: Specific scenario description
    Given initial state
    When action is performed
    Then expected result occurs
```

### 2. Tagging Strategy

Use meaningful tags for test categorization:

- **Browser**: `@chrome`, `@firefox`, `@edge`
- **Type**: `@smoke`, `@regression`, `@sanity`
- **Feature**: `@login`, `@checkout`, `@profile`
- **Priority**: `@critical`, `@high`, `@medium`, `@low`
- **Data**: `@database` (for tests requiring database setup)

Example:
```gherkin
@chrome @smoke @login @critical
Scenario: Critical login test in Chrome
```

### 3. Step Definition Best Practices

```csharp
// GOOD: Specific, reusable steps
[Given(@"I am logged in as an admin")]
[When(@"I click on the ""(.*)"" button")]
[Then(@"I should see the error message ""(.*)""")]

// AVOID: Overly generic steps
[When(@"I do something")]
[Then(@"Something happens")]

// GOOD: Parameterized steps
[When(@"I enter ""(.*)"" in the ""(.*)"" field")]
public void WhenIEnterInTheField(string value, string fieldName)
{
    _page.EnterText(GetFieldXPath(fieldName), value);
}

// GOOD: Table parameters for complex data
[Given(@"I have the following users:")]
public void GivenIHaveTheFollowingUsers(Table table)
{
    foreach (var row in table.Rows)
    {
        var username = row["Username"];
        var email = row["Email"];
        // Create user
    }
}
```

### 4. Page Object Guidelines

```csharp
public class ExamplePage : BasePage
{
    // Group related locators
    #region Locators
    private const string HeaderXPath = "//header[@id='main-header']";
    private const string LoginButtonXPath = "//button[@id='login']";
    private const string UsernameFieldXPath = "//input[@name='username']";
    #endregion

    public ExamplePage(SeleniumHooks seleniumHooks) : base(seleniumHooks)
    {
    }

    // Implement required method
    public override bool IsPageLoaded()
    {
        WaitForPageLoad();
        WaitForAngularLoad();
        return IsElementDisplayed(HeaderXPath);
    }

    // Public action methods
    public void Login(string username, string password)
    {
        EnterText(UsernameFieldXPath, username);
        EnterText(PasswordFieldXPath, password);
        ClickElement(LoginButtonXPath);
        WaitForAngularLoad();
    }

    // Assertion methods
    public void AssertLoginSuccessful()
    {
        WaitForElement(WelcomeMessageXPath, 30);
        IsElementDisplayed(WelcomeMessageXPath)
            .ShouldBeTrue("Welcome message should be displayed after login");
    }

    // Return page objects for navigation
    public DashboardPage NavigateToDashboard()
    {
        ClickElement(DashboardLinkXPath);
        return new DashboardPage(SeleniumHooks);
    }
}
```

## Running Tests

### Command Line Execution

```bash
# Run all tests
dotnet test

# Run with specific configuration
dotnet test -c Release

# Run ReqNRoll tests with specific tag
dotnet test --filter "Category=smoke"

# Run NUnit tests with specific category
dotnet test --filter "TestCategory=Regression"

# Run tests in parallel (NUnit)
dotnet test -- NUnit.NumberOfTestWorkers=4

# Run with detailed output
dotnet test -v detailed

# Generate test results
dotnet test --logger "trx;LogFileName=testresults.trx"
```

### Visual Studio Test Explorer

1. Build the solution
2. Open **Test Explorer** (Test → Test Explorer)
3. Group by: Category, Trait, Class, etc.
4. Right-click tests or groups to run
5. View test results and output

### Filtering Tests

**By Tag (ReqNRoll):**
```bash
dotnet test --filter "Category=smoke&Category=chrome"
```

**By Category (NUnit):**
```bash
dotnet test --filter "TestCategory=Smoke"
```

**By Name:**
```bash
dotnet test --filter "FullyQualifiedName~AngularHomePage"
```

## Test Data Management

### Using DataSetup Project

```csharp
[Binding]
public class DatabaseHooks
{
    private DataSetupManager _dataManager;

    [BeforeScenario("@database")]
    public void SetupTestData()
    {
        var config = TestConfiguration.Instance;
        _dataManager = new DataSetupManager(config.ConnectionString);
        _dataManager.ExecuteSetupScript("SampleData/CreateTestUsers.sql");
    }

    [AfterScenario("@database")]
    public void CleanupTestData()
    {
        _dataManager.ExecuteSetupScript("Cleanup/CleanupTestUsers.sql");
    }
}
```

**Tag scenarios requiring database:**
```gherkin
@database @chrome
Scenario: Test with database
    Given I have test data in the database
    When I perform an action
    Then the data should be updated
```

### Using Test Data in Steps

```csharp
[Given(@"I have the following products:")]
public void GivenIHaveTheFollowingProducts(Table table)
{
    foreach (var row in table.Rows)
    {
        var productName = row["Name"];
        var price = row["Price"];
        _productPage.AddProduct(productName, price);
    }
}
```

**In feature file:**
```gherkin
Given I have the following products:
    | Name          | Price |
    | Product A     | 10.99 |
    | Product B     | 20.99 |
```

## Reporting and Screenshots

### Screenshot on Failure

Screenshots are automatically captured when tests fail (configured in `TestHooks.cs`):

```csharp
[AfterScenario]
public void AfterScenario()
{
    if (_scenarioContext.TestError != null && _config.ScreenshotOnFailure)
    {
        var scenarioTitle = _scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
        _seleniumHooks?.TakeScreenshot($"Failed_{scenarioTitle}");
    }
}
```

Screenshots are saved to: `Screenshots/` directory

### Manual Screenshots

```csharp
// In page object
public void TakePageScreenshot(string name)
{
    TakeScreenshot($"PageName_{name}");
}

// In test
[Then(@"I take a screenshot of the page")]
public void ThenITakeAScreenshot()
{
    _page.TakePageScreenshot("custom_name");
}
```

### Logging

All actions are automatically logged:

```
[2025-01-04 10:30:15] INFO: Starting scenario: Verify Angular home page loads
[2025-01-04 10:30:16] INFO: Initialized Chrome browser
[2025-01-04 10:30:17] INFO: Navigating to URL: https://angular.io
[2025-01-04 10:30:18] INFO: Waiting for Angular page to load...
[2025-01-04 10:30:19] INFO: Angular page loaded successfully
[2025-01-04 10:30:20] INFO: Completed scenario: Verify Angular home page loads
```

Logs are saved to: `Logs/TestLog_YYYYMMDD.txt`

## Configuration

Edit `appsettings.json` in the UITests project:

```json
{
  "TestSettings": {
    "BaseUrl": "https://your-application.com",
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
  },
  "Logging": {
    "LogLevel": "Information",
    "LogPath": "Logs"
  }
}
```

### Environment-Specific Configuration

Create multiple configuration files:
- `appsettings.json` (default)
- `appsettings.Development.json`
- `appsettings.Staging.json`
- `appsettings.Production.json`

## Troubleshooting

### Common Issues

**Issue: Step definition not found**
- Rebuild the solution
- Check `[Binding]` attribute on step class
- Verify step regex matches feature file

**Issue: ScenarioContext is null**
- Ensure ReqNRoll.NUnit package is installed
- Check constructor injection in step definitions

**Issue: Element not found**
- Verify XPath with browser DevTools
- Add explicit waits
- Check for Angular loading

**Issue: Tests not discovered**
- Rebuild solution
- Clean solution and rebuild
- Check test framework packages

### Debug Mode

Run tests with detailed logging:
```bash
dotnet test -v detailed
```

Set breakpoints in:
- Step definitions
- Page object methods
- Hook methods

## Best Practices Summary

1. **One assertion per test** (or related assertions)
2. **Use meaningful test names** that describe what is being tested
3. **Follow AAA pattern**: Arrange, Act, Assert
4. **Keep tests independent** - no dependencies between tests
5. **Use Page Objects** - never use Selenium directly in tests
6. **Tag appropriately** for easy filtering and organization
7. **Clean up test data** in [TearDown] or [AfterScenario]
8. **Use explicit waits** instead of Thread.Sleep
9. **Handle Angular loading** for Angular applications
10. **Take screenshots on failure** for debugging

---

**For more information**, refer to the main [README.md](../README.md)
