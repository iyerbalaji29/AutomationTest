using Common.Configuration;
using Common.Enums;
using Common.Selenium.Implementations;
using Common.Selenium.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Common.Selenium
{
    /// <summary>
    /// Central Selenium hooks providing common WebDriver operations
    /// Follows Facade pattern to simplify WebDriver interactions
    /// All XPath-based element operations go through this class
    /// </summary>
    public class SeleniumHooks : IElementActions
    {
        protected IWebDriver? Driver;
        private readonly TestConfiguration _config;
        private readonly IElementLocator _elementLocator;
        private readonly IWebDriverFactory _driverFactory;

        public SeleniumHooks()
        {
            _config = TestConfiguration.Instance;
            _elementLocator = new XPathElementLocator();
            _driverFactory = new WebDriverFactory();
        }

        public IWebDriver GetDriver()
        {
            if (Driver == null)
                throw new InvalidOperationException("WebDriver is not initialized. Call InitializeDriver first.");
            return Driver;
        }

        public void InitializeDriver(BrowserType browserType)
        {
            Driver = _driverFactory.CreateDriver(browserType);
        }

        public void InitializeDriver(string browserName)
        {
            Driver = _driverFactory.CreateDriver(browserName);
        }

        public void QuitDriver()
        {
            try
            {
                Driver?.Quit();
                Driver?.Dispose();
                Driver = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while quitting driver: {ex.Message}");
            }
        }

        // Element Actions using XPath by default
        public void Click(string locator)
        {
            var element = _elementLocator.FindElement(GetDriver(), locator);
            element.Click();
        }

        public void SendKeys(string locator, string text)
        {
            var element = _elementLocator.FindElement(GetDriver(), locator);
            element.Clear();
            element.SendKeys(text);
        }

        public void Clear(string locator)
        {
            var element = _elementLocator.FindElement(GetDriver(), locator);
            element.Clear();
        }

        public string GetText(string locator)
        {
            var element = _elementLocator.FindElement(GetDriver(), locator);
            return element.Text;
        }

        public string GetAttribute(string locator, string attributeName)
        {
            var element = _elementLocator.FindElement(GetDriver(), locator);
            return element.GetAttribute(attributeName) ?? string.Empty;
        }

        public bool IsDisplayed(string locator)
        {
            try
            {
                var element = _elementLocator.FindElement(GetDriver(), locator);
                return element.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsEnabled(string locator)
        {
            var element = _elementLocator.FindElement(GetDriver(), locator);
            return element.Enabled;
        }

        public bool IsSelected(string locator)
        {
            var element = _elementLocator.FindElement(GetDriver(), locator);
            return element.Selected;
        }

        public void WaitForElementVisible(string locator, int timeoutSeconds = 30)
        {
            var wait = new WebDriverWait(GetDriver(), TimeSpan.FromSeconds(timeoutSeconds));
            wait.Until(driver => _elementLocator.FindElement(driver, locator).Displayed);
        }

        public void WaitForElementClickable(string locator, int timeoutSeconds = 30)
        {
            var wait = new WebDriverWait(GetDriver(), TimeSpan.FromSeconds(timeoutSeconds));
            wait.Until(driver =>
            {
                var element = _elementLocator.FindElement(driver, locator);
                return element.Displayed && element.Enabled;
            });
        }

        public IWebElement FindElement(string xpath)
        {
            return _elementLocator.FindElement(GetDriver(), xpath);
        }

        public IList<IWebElement> FindElements(string xpath)
        {
            return _elementLocator.FindElements(GetDriver(), xpath);
        }

        // Navigation methods
        public void NavigateTo(string url)
        {
            GetDriver().Navigate().GoToUrl(url);
        }

        public void NavigateBack()
        {
            GetDriver().Navigate().Back();
        }

        public void NavigateForward()
        {
            GetDriver().Navigate().Forward();
        }

        public void Refresh()
        {
            GetDriver().Navigate().Refresh();
        }

        // JavaScript execution
        public object ExecuteJavaScript(string script, params object[] args)
        {
            var jsExecutor = (IJavaScriptExecutor)GetDriver();
            return jsExecutor.ExecuteScript(script, args);
        }

        // Screenshot capability
        public void TakeScreenshot(string fileName)
        {
            var screenshotDriver = GetDriver() as ITakesScreenshot;
            if (screenshotDriver == null) return;

            var screenshot = screenshotDriver.GetScreenshot();
            var screenshotPath = Path.Combine(_config.ScreenshotPath, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            Directory.CreateDirectory(_config.ScreenshotPath);
            screenshot.SaveAsFile(screenshotPath);
        }

        // Wait methods
        public void WaitForPageLoad(int timeoutSeconds = 30)
        {
            var wait = new WebDriverWait(GetDriver(), TimeSpan.FromSeconds(timeoutSeconds));
            wait.Until(driver => ((IJavaScriptExecutor)driver)
                .ExecuteScript("return document.readyState").ToString() == "complete");
        }

        // Angular-specific wait
        public void WaitForAngular(int timeoutSeconds = 30)
        {
            var wait = new WebDriverWait(GetDriver(), TimeSpan.FromSeconds(timeoutSeconds));
            wait.Until(driver =>
            {
                var angularReady = (bool)((IJavaScriptExecutor)driver).ExecuteScript(
                    @"return window.getAllAngularTestabilities &&
                      window.getAllAngularTestabilities().findIndex(x => !x.isStable()) === -1");
                return angularReady;
            });
        }

        public bool IsAngularPageLoaded()
        {
            try
            {
                WaitForAngular();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
