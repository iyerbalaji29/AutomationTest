# Common Project

The Common project is the core framework library containing shared utilities, Selenium automation framework, configuration management, and page object base classes.

## Purpose

This project provides:
- **Selenium WebDriver** abstraction and utilities
- **Page Object Model** base classes
- **Configuration management**
- **Logging infrastructure**
- **Browser driver factory**
- **Element location framework**
- **Extension methods**

## Architecture

```
Common/
├── Configuration/           # Configuration management
├── Enums/                  # Shared enumerations
├── Selenium/               # Selenium framework
│   ├── Interfaces/         # Abstraction contracts
│   ├── Implementations/    # Concrete implementations
│   └── SeleniumHooks.cs   # Central Selenium operations
├── PageObjects/            # Page Object base classes
├── Utilities/              # Utility classes
└── Extensions/             # Extension methods
```

## Key Components

### Configuration Management

#### TestConfiguration.cs
Singleton configuration manager providing centralized access to settings.

**Features:**
- Singleton pattern for global access
- JSON configuration file support
- Type-safe property access
- Runtime configuration updates

**Usage:**
```csharp
var config = TestConfiguration.Instance;
var baseUrl = config.BaseUrl;
var browser = config.Browser;
var timeout = config.ExplicitWaitSeconds;
```

**Configuration Properties:**
```csharp
// Test Settings
BaseUrl                     // Application URL
Browser                     // Default browser (Chrome, Firefox, Edge)
ImplicitWaitSeconds        // Implicit wait timeout
ExplicitWaitSeconds        // Explicit wait timeout
PageLoadTimeoutSeconds     // Page load timeout
HeadlessMode               // Run browsers headless
ScreenshotOnFailure        // Capture screenshots on failure
ScreenshotPath             // Screenshot storage location

// Database Settings
ConnectionString           // Database connection string

// Logging Settings
LogLevel                   // Logging level (Debug, Info, Warning, Error)
LogPath                    // Log file location
```

### Selenium Framework

#### SeleniumHooks.cs
Central class for all Selenium WebDriver operations. This is the **primary interface** for browser automation.

**Responsibilities:**
- WebDriver lifecycle management
- Element location and interaction
- Waiting strategies
- JavaScript execution
- Screenshot capture
- Angular-specific operations

**Key Methods:**

```csharp
// Driver Management
InitializeDriver(BrowserType browserType)
InitializeDriver(string browserName)
QuitDriver()
GetDriver()

// Navigation
NavigateTo(string url)
NavigateBack()
NavigateForward()
Refresh()

// Element Actions (XPath-based)
Click(string xpath)
SendKeys(string xpath, string text)
Clear(string xpath)
GetText(string xpath)
GetAttribute(string xpath, string attributeName)
IsDisplayed(string xpath)
IsEnabled(string xpath)
IsSelected(string xpath)

// Waiting
WaitForElementVisible(string xpath, int timeoutSeconds = 30)
WaitForElementClickable(string xpath, int timeoutSeconds = 30)
WaitForPageLoad(int timeoutSeconds = 30)
WaitForAngular(int timeoutSeconds = 30)
IsAngularPageLoaded()

// Element Location
FindElement(string xpath)
FindElements(string xpath)

// Utilities
ExecuteJavaScript(string script, params object[] args)
TakeScreenshot(string fileName)
```

**Example Usage:**
```csharp
var hooks = new SeleniumHooks();
hooks.InitializeDriver(BrowserType.Chrome);
hooks.NavigateTo("https://example.com");
hooks.WaitForAngular();
hooks.Click("//button[@id='submit']");
hooks.QuitDriver();
```

#### WebDriverFactory
Factory class for creating browser driver instances.

**Supported Browsers:**
- Chrome (with ChromeDriver)
- Firefox (with GeckoDriver)
- Edge (with EdgeDriver)

**Features:**
- Automatic driver configuration
- Headless mode support
- Browser-specific options
- Window maximization
- Implicit/explicit timeout configuration

**Implementation:**
```csharp
public class WebDriverFactory : IWebDriverFactory
{
    public IWebDriver CreateDriver(BrowserType browserType)
    {
        return browserType switch
        {
            BrowserType.Chrome => CreateChromeDriver(),
            BrowserType.Firefox => CreateFirefoxDriver(),
            BrowserType.Edge => CreateEdgeDriver(),
            _ => throw new ArgumentException($"Unsupported browser: {browserType}")
        };
    }
}
```

**Adding a New Browser:**

```csharp
// 1. Add to BrowserType enum
public enum BrowserType
{
    Chrome,
    Firefox,
    Edge,
    Safari  // New browser
}

// 2. Add case to WebDriverFactory
private IWebDriver CreateSafariDriver()
{
    var options = new SafariOptions();
    // Configure options
    var driver = new SafariDriver(options);
    ConfigureDriver(driver);
    return driver;
}

// 3. Update CreateDriver switch
BrowserType.Safari => CreateSafariDriver(),
```

#### Element Location Framework

The framework uses **XPath as the primary locator strategy** with extensibility for other strategies.

**XPathElementLocator:**
```csharp
public class XPathElementLocator : IElementLocator
{
    public By GetLocator(string locatorValue)
    {
        return By.XPath(locatorValue);
    }

    public IWebElement FindElement(IWebDriver driver, string locatorValue)
    {
        return driver.FindElement(GetLocator(locatorValue));
    }
}
```

**ElementLocatorFactory:**
Supports multiple locator strategies through factory pattern.

```csharp
// Get By locator
var by = ElementLocatorFactory.GetBy("//div[@id='content']", LocatorType.XPath);

// Get locator implementation
var locator = ElementLocatorFactory.GetLocator(LocatorType.XPath);
var element = locator.FindElement(driver, "//button[@id='submit']");
```

**Supported Locator Types:**
- XPath (default)
- Id
- Name
- ClassName
- CssSelector
- TagName
- LinkText
- PartialLinkText

### Page Object Model

#### AbstractPage
Base abstract class defining the template for all page objects.

```csharp
public abstract class AbstractPage
{
    protected readonly SeleniumHooks SeleniumHooks;
    protected IWebDriver Driver => SeleniumHooks.GetDriver();

    protected AbstractPage(SeleniumHooks seleniumHooks)
    {
        SeleniumHooks = seleniumHooks;
    }

    // Must be implemented by derived classes
    public abstract bool IsPageLoaded();

    // Common page methods
    public virtual string GetPageTitle();
    public virtual string GetCurrentUrl();
    public virtual void WaitForPageLoad();
    public virtual void RefreshPage();
}
```

#### BasePage
Concrete base class with common functionality, inherits from AbstractPage.

**Features:**
- Navigation management
- Angular page loading verification
- Element interaction helpers
- Screenshot capability
- JavaScript execution
- Logging integration

**Common Methods:**
```csharp
// Navigation
NavigateTo(string url)
WaitForAngularLoad()
IsAngularPageLoadedSuccessfully()

// Element Interactions (protected, for derived classes)
ClickElement(string xpath)
EnterText(string xpath, string text)
GetElementText(string xpath)
IsElementDisplayed(string xpath)
WaitForElement(string xpath, int timeoutSeconds = 30)

// Utilities
TakeScreenshot(string fileName)
ExecuteScript(string script, params object[] args)
```

**Creating a New Page Object:**

```csharp
public class LoginPage : BasePage
{
    // Locators
    private const string UsernameFieldXPath = "//input[@id='username']";
    private const string PasswordFieldXPath = "//input[@id='password']";
    private const string LoginButtonXPath = "//button[@type='submit']";

    public LoginPage(SeleniumHooks seleniumHooks) : base(seleniumHooks)
    {
    }

    public override bool IsPageLoaded()
    {
        WaitForPageLoad();
        return IsElementDisplayed(UsernameFieldXPath);
    }

    public void Login(string username, string password)
    {
        EnterText(UsernameFieldXPath, username);
        EnterText(PasswordFieldXPath, password);
        ClickElement(LoginButtonXPath);
        WaitForAngularLoad();
    }

    public void AssertLoginPageDisplayed()
    {
        IsPageLoaded().ShouldBeTrue("Login page should be displayed");
    }
}
```

### Utilities

#### Logger
Centralized logging using Serilog.

**Features:**
- Console and file logging
- Daily log rotation
- Multiple log levels
- Exception logging

**Usage:**
```csharp
Logger.Info("Test started");
Logger.Debug("Debug information");
Logger.Warning("Warning message");
Logger.Error("Error occurred", exception);
Logger.Fatal("Fatal error", exception);
```

**Log Output:**
- Console: Real-time feedback
- File: `Logs/TestLog_YYYYMMDD.txt`

### Extensions

#### WebDriverExtensions
Extension methods for IWebDriver.

**Methods:**
```csharp
driver.WaitForCondition(d => d.Title.Contains("Expected"), 30);
driver.ScrollToElement(element);
driver.ScrollToBottom();
driver.ScrollToTop();
driver.HighlightElement(element);
driver.SwitchToFrame("frameName");
driver.SwitchToDefaultContent();
driver.AcceptAlert();
driver.DismissAlert();
driver.GetAlertText();
```

**Example:**
```csharp
var driver = _hooks.GetDriver();
driver.ScrollToBottom();
driver.AcceptAlert();
```

#### WebElementExtensions
Extension methods for IWebElement.

**Methods:**
```csharp
element.ClickWithJavaScript(driver);
element.SendKeysSlowly("text", delayMs: 100);
element.HoverOver(driver);
element.DoubleClick(driver);
element.RightClick(driver);
element.HasClass("active");
element.WaitUntilVisible(timeoutSeconds: 30);
```

**Example:**
```csharp
var submitButton = driver.FindElement(By.XPath("//button[@id='submit']"));
submitButton.HoverOver(driver);
submitButton.ClickWithJavaScript(driver);
```

## Angular-Specific Features

### Angular Page Detection

The framework includes built-in Angular detection:

```csharp
// Wait for Angular to be ready
SeleniumHooks.WaitForAngular();

// Check if Angular page loaded
bool isLoaded = SeleniumHooks.IsAngularPageLoaded();
```

**Implementation:**
```javascript
// Checks if all Angular testabilities are stable
return window.getAllAngularTestabilities &&
       window.getAllAngularTestabilities().findIndex(x => !x.isStable()) === -1;
```

### Using in Page Objects

```csharp
public override bool IsPageLoaded()
{
    WaitForPageLoad();
    WaitForAngularLoad();  // Wait for Angular
    return IsElementDisplayed(PageIdentifierXPath);
}
```

## Best Practices

### 1. Page Object Design

```csharp
// GOOD: Encapsulated, single responsibility
public class ProductPage : BasePage
{
    private const string AddToCartButtonXPath = "//button[@data-test='add-to-cart']";

    public void AddToCart()
    {
        WaitForElement(AddToCartButtonXPath);
        ClickElement(AddToCartButtonXPath);
    }

    public void AssertProductAdded()
    {
        IsElementDisplayed("//div[@class='cart-notification']")
            .ShouldBeTrue("Cart notification should be displayed");
    }
}
```

### 2. XPath Best Practices

```csharp
// GOOD: Specific, maintainable
private const string SubmitButton = "//button[@id='submit' and @type='submit']";
private const string ErrorMessage = "//div[@class='error-message' and contains(text(), 'Invalid')]";

// AVOID: Brittle, position-dependent
private const string SubmitButton = "//button[3]";
private const string ErrorMessage = "//div[1]/span[2]";

// GOOD: Relative XPath
private const string UsernameField = "//form[@id='login']//input[@name='username']";

// GOOD: Using attributes
private const string NavLink = "//a[@data-test-id='navigation-home']";
```

### 3. Wait Strategies

```csharp
// GOOD: Explicit waits
WaitForElement(ElementXPath, 30);
ClickElement(ElementXPath);

// AVOID: Thread.Sleep
Thread.Sleep(5000);  // Don't use this
ClickElement(ElementXPath);

// GOOD: Custom conditions
driver.WaitForCondition(d => d.Title.Contains("Dashboard"), 30);
```

### 4. Logging

```csharp
public void PerformAction()
{
    Logger.Info("Starting action");

    try
    {
        ClickElement(ButtonXPath);
        Logger.Info("Action completed successfully");
    }
    catch (Exception ex)
    {
        Logger.Error("Action failed", ex);
        throw;
    }
}
```

## Configuration

Edit `appsettings.json`:

```json
{
  "TestSettings": {
    "BaseUrl": "https://your-app.com",
    "Browser": "Chrome",
    "ImplicitWaitSeconds": 10,
    "ExplicitWaitSeconds": 30,
    "PageLoadTimeoutSeconds": 60,
    "HeadlessMode": false,
    "ScreenshotOnFailure": true,
    "ScreenshotPath": "Screenshots"
  },
  "Logging": {
    "LogLevel": "Information",
    "LogPath": "Logs"
  }
}
```

## Dependencies

- **Selenium.WebDriver** (4.18.1): Browser automation
- **Selenium.Support** (4.18.1): Additional Selenium utilities
- **NUnit** (4.1.0): Testing framework
- **Shouldly** (4.2.1): Assertion library
- **Serilog** (3.1.1): Logging framework
- **Microsoft.Extensions.Configuration** (8.0.0): Configuration management

## Extending the Framework

### Adding Custom Wait Conditions

```csharp
// In WebDriverExtensions.cs
public static void WaitForTextToContain(
    this IWebDriver driver,
    string xpath,
    string expectedText,
    int timeoutSeconds = 30)
{
    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
    wait.Until(d =>
    {
        var element = d.FindElement(By.XPath(xpath));
        return element.Text.Contains(expectedText);
    });
}
```

### Adding Custom Element Actions

```csharp
// In SeleniumHooks.cs
public void SelectDropdownByText(string xpath, string text)
{
    var element = FindElement(xpath);
    var select = new SelectElement(element);
    select.SelectByText(text);
}
```

## Troubleshooting

**Issue: "Driver not found"**
- Selenium 4.6+ uses Selenium Manager (automatic)
- Manually download drivers if needed

**Issue: "Element not found"**
- Verify XPath with browser DevTools: `$x("your-xpath")`
- Add explicit waits
- Check for iframes

**Issue: "Angular not detected"**
- Verify page uses Angular
- Increase timeout in `WaitForAngular()`
- Check browser console for errors

---

**For more information**, refer to the main [README.md](../README.md)
