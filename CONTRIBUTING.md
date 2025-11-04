# Contributing Guide

Thank you for your interest in contributing to the Automation Test Framework! This guide will help you understand how to add new features and maintain the framework.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Adding New Features](#adding-new-features)
- [Coding Standards](#coding-standards)
- [Testing Guidelines](#testing-guidelines)
- [Documentation](#documentation)
- [Pull Request Process](#pull-request-process)

## Code of Conduct

- Be respectful and professional
- Follow SOLID principles
- Write clean, maintainable code
- Document your changes
- Test thoroughly before submitting

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or VS Code
- Git knowledge
- Understanding of C#, Selenium, and design patterns

### Setting Up Development Environment

1. Clone the repository
2. Open solution in Visual Studio
3. Restore NuGet packages
4. Build the solution
5. Run existing tests to ensure everything works

## Development Workflow

### Branch Strategy

```
main
  └── develop
       ├── feature/your-feature-name
       ├── bugfix/issue-description
       └── enhancement/improvement-description
```

### Creating a Feature Branch

```bash
git checkout develop
git pull origin develop
git checkout -b feature/your-feature-name
```

## Adding New Features

### Adding a New Page Object

**1. Create the Page Class**

Location: `UITests/Pages/YourPage.cs`

```csharp
using Common.PageObjects;
using Common.Selenium;
using Shouldly;

namespace UITests.Pages
{
    /// <summary>
    /// Page Object for [Page Name]
    /// [Brief description of the page]
    /// </summary>
    public class YourPage : BasePage
    {
        #region Locators
        // Group all XPath locators here
        private const string ElementXPath = "//element[@id='example']";
        private const string ButtonXPath = "//button[@id='submit']";
        #endregion

        public YourPage(SeleniumHooks seleniumHooks) : base(seleniumHooks)
        {
        }

        public override bool IsPageLoaded()
        {
            WaitForPageLoad();
            WaitForAngularLoad();
            return IsElementDisplayed(ElementXPath);
        }

        #region Actions
        public void PerformAction()
        {
            WaitForElement(ButtonXPath);
            ClickElement(ButtonXPath);
        }
        #endregion

        #region Assertions
        public void AssertElementDisplayed()
        {
            IsElementDisplayed(ElementXPath)
                .ShouldBeTrue("Element should be displayed");
        }
        #endregion

        #region Navigation
        public NextPage NavigateToNextPage()
        {
            ClickElement(ButtonXPath);
            return new NextPage(SeleniumHooks);
        }
        #endregion
    }
}
```

**2. Add Tests for the Page**

BDD approach (`UITests/Features/YourFeature.feature`):
```gherkin
Feature: Your Feature
    Description of what this feature tests

@chrome @smoke
Scenario: Test scenario
    Given I navigate to your page
    When I perform an action
    Then I should see the result
```

**3. Create Step Definitions**

```csharp
[Binding]
public class YourFeatureSteps
{
    private readonly YourPage _yourPage;

    public YourFeatureSteps(ScenarioContext scenarioContext)
    {
        var seleniumHooks = (SeleniumHooks)scenarioContext["SeleniumHooks"];
        _yourPage = new YourPage(seleniumHooks);
    }

    [Given(@"I navigate to your page")]
    public void GivenINavigateToYourPage()
    {
        _yourPage.NavigateToPage();
    }
}
```

### Adding a New Browser

**1. Update BrowserType Enum**

Location: `Common/Enums/BrowserType.cs`

```csharp
public enum BrowserType
{
    Chrome,
    Firefox,
    Edge,
    Safari  // Add new browser
}
```

**2. Update WebDriverFactory**

Location: `Common/Selenium/Implementations/WebDriverFactory.cs`

```csharp
public IWebDriver CreateDriver(BrowserType browserType)
{
    return browserType switch
    {
        BrowserType.Chrome => CreateChromeDriver(),
        BrowserType.Firefox => CreateFirefoxDriver(),
        BrowserType.Edge => CreateEdgeDriver(),
        BrowserType.Safari => CreateSafariDriver(),  // Add case
        _ => throw new ArgumentException($"Unsupported browser: {browserType}")
    };
}

private IWebDriver CreateSafariDriver()
{
    var options = new SafariOptions();
    // Configure options
    var driver = new SafariDriver(options);
    ConfigureDriver(driver);
    return driver;
}
```

**3. Add NuGet Package** (if needed)

```xml
<PackageReference Include="Selenium.WebDriver.SafariDriver" Version="x.x.x" />
```

### Adding a New Locator Strategy

**1. Create Locator Implementation**

Location: `Common/Selenium/Implementations/CssElementLocator.cs`

```csharp
using Common.Selenium.Interfaces;
using OpenQA.Selenium;

namespace Common.Selenium.Implementations
{
    public class CssElementLocator : IElementLocator
    {
        public By GetLocator(string locatorValue)
        {
            return By.CssSelector(locatorValue);
        }

        public IWebElement FindElement(IWebDriver driver, string locatorValue)
        {
            return driver.FindElement(GetLocator(locatorValue));
        }

        public IList<IWebElement> FindElements(IWebDriver driver, string locatorValue)
        {
            return driver.FindElements(GetLocator(locatorValue));
        }
    }
}
```

**2. Update ElementLocatorFactory**

```csharp
public static IElementLocator GetLocator(LocatorType locatorType)
{
    return locatorType switch
    {
        LocatorType.XPath => new XPathElementLocator(),
        LocatorType.CssSelector => new CssElementLocator(),  // Add
        _ => new XPathElementLocator()
    };
}
```

### Adding Extension Methods

**Location: `Common/Extensions/`**

```csharp
public static class WebDriverExtensions
{
    public static void YourNewMethod(this IWebDriver driver, params)
    {
        // Implementation
    }
}

public static class WebElementExtensions
{
    public static void YourNewMethod(this IWebElement element, params)
    {
        // Implementation
    }
}
```

## Coding Standards

### C# Coding Conventions

1. **Naming Conventions**
   ```csharp
   // Classes, Methods, Properties: PascalCase
   public class MyClass { }
   public void MyMethod() { }
   public string MyProperty { get; set; }

   // Private fields: _camelCase
   private string _myField;

   // Constants: PascalCase
   private const string MyConstant = "value";

   // Parameters, local variables: camelCase
   public void Method(string myParameter)
   {
       var localVariable = "";
   }
   ```

2. **File Organization**
   ```csharp
   // Using statements
   using System;
   using Common.Selenium;

   // Namespace
   namespace UITests.Pages
   {
       /// <summary>
       /// XML documentation
       /// </summary>
       public class MyClass
       {
           #region Fields
           #endregion

           #region Properties
           #endregion

           #region Constructors
           #endregion

           #region Public Methods
           #endregion

           #region Private Methods
           #endregion
       }
   }
   ```

3. **SOLID Principles**
   - **S**ingle Responsibility: One class, one purpose
   - **O**pen/Closed: Open for extension, closed for modification
   - **L**iskov Substitution: Derived classes must be substitutable
   - **I**nterface Segregation: Many specific interfaces > one general
   - **D**ependency Inversion: Depend on abstractions, not concretions

4. **XML Documentation**
   ```csharp
   /// <summary>
   /// Brief description of what this does
   /// </summary>
   /// <param name="paramName">Description of parameter</param>
   /// <returns>Description of return value</returns>
   /// <exception cref="ExceptionType">When this exception is thrown</exception>
   public string MyMethod(string paramName)
   {
       // Implementation
   }
   ```

### XPath Best Practices

```csharp
// GOOD: Specific, maintainable
private const string SubmitButton = "//button[@id='submit' and @type='submit']";

// GOOD: Using data attributes
private const string NavItem = "//a[@data-testid='nav-home']";

// GOOD: Relative paths
private const string FormInput = "//form[@id='login']//input[@name='username']";

// AVOID: Position-dependent
private const string BadLocator = "//div[3]/span[1]";  // Brittle!

// AVOID: Too generic
private const string BadLocator = "//button";  // Which button?
```

### Test Writing Standards

```csharp
[Test]
[Description("Clear description of what is being tested")]
[Category("Smoke")]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var page = new MyPage(_seleniumHooks);

    // Act
    page.PerformAction();

    // Assert
    page.AssertExpectedResult();
}
```

### Feature File Standards

```gherkin
Feature: Feature Name
    As a [role]
    I want to [action]
    So that I can [benefit]

Background:
    Given common setup for all scenarios

@tag1 @tag2
Scenario: Clear scenario description
    Given initial state
    When action is performed
    Then expected outcome occurs
    And additional verification
```

## Testing Guidelines

### Unit Test Coverage

- All new methods should have corresponding tests
- Aim for >80% code coverage
- Test both happy path and error cases

### Integration Tests

- Test interaction between components
- Verify end-to-end workflows
- Test multi-browser scenarios

### Test Independence

- Each test should run independently
- No shared state between tests
- Clean up after each test

## Documentation

### Required Documentation

1. **XML Documentation** for all public methods
2. **README updates** if adding new features
3. **Inline comments** for complex logic
4. **Example usage** for new utilities

### Documentation Standards

```csharp
/// <summary>
/// Clicks on the specified element after waiting for it to be clickable
/// </summary>
/// <param name="xpath">XPath locator of the element</param>
/// <exception cref="NoSuchElementException">When element is not found</exception>
/// <example>
/// <code>
/// ClickElement("//button[@id='submit']");
/// </code>
/// </example>
public void ClickElement(string xpath)
{
    // Wait for element to be clickable
    WaitForElementClickable(xpath);

    // Perform click action
    _elementLocator.FindElement(GetDriver(), xpath).Click();
}
```

## Pull Request Process

### Before Submitting

1. **Run all tests**
   ```bash
   dotnet test
   ```

2. **Check code formatting**
   - Follow C# conventions
   - Use consistent indentation (4 spaces)
   - Remove unused using statements

3. **Update documentation**
   - Add/update XML comments
   - Update README if needed
   - Add examples for new features

4. **Review your changes**
   - Self-review code
   - Check for commented code
   - Verify naming conventions

### Creating Pull Request

1. **Commit your changes**
   ```bash
   git add .
   git commit -m "feat: Add new page object for login page"
   ```

   **Commit Message Format:**
   ```
   type: subject

   body (optional)

   footer (optional)
   ```

   **Types:**
   - `feat`: New feature
   - `fix`: Bug fix
   - `docs`: Documentation only
   - `style`: Code style changes
   - `refactor`: Code refactoring
   - `test`: Adding tests
   - `chore`: Maintenance

2. **Push to your branch**
   ```bash
   git push origin feature/your-feature-name
   ```

3. **Create Pull Request**
   - Clear title describing the change
   - Description of what changed and why
   - Reference any related issues
   - Include test results
   - Add screenshots if UI changes

### Pull Request Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] All tests pass
- [ ] Added new tests
- [ ] Manual testing completed

## Checklist
- [ ] Code follows project standards
- [ ] Self-reviewed code
- [ ] Updated documentation
- [ ] No breaking changes (or documented)
```

### Review Process

1. Automated tests must pass
2. Code review by maintainer
3. Address review comments
4. Approval and merge

## Version Control

### Branch Naming

```
feature/short-description
bugfix/issue-number-description
enhancement/improvement-name
```

### Commit Messages

```
feat: Add login page object
fix: Correct XPath for submit button
docs: Update README with new examples
test: Add tests for navigation functionality
```

## Maintenance

### Regular Tasks

1. **Update dependencies**
   ```bash
   dotnet list package --outdated
   ```

2. **Review and refactor**
   - Remove unused code
   - Improve test coverage
   - Update documentation

3. **Monitor performance**
   - Check test execution time
   - Optimize slow tests
   - Review screenshots and logs

## Getting Help

- Review existing code and patterns
- Check project README files
- Ask questions in pull requests
- Consult SOLID principles documentation

## Recognition

Contributors will be:
- Listed in CONTRIBUTORS.md
- Recognized in release notes
- Credited for significant features

Thank you for contributing to making this framework better!
