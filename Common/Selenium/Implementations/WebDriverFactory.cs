using Common.Configuration;
using Common.Enums;
using Common.Selenium.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace Common.Selenium.Implementations
{
    /// <summary>
    /// WebDriver factory implementation
    /// Follows Factory Pattern and creates browser-specific drivers
    /// </summary>
    public class WebDriverFactory : IWebDriverFactory
    {
        private readonly TestConfiguration _config;

        public WebDriverFactory()
        {
            _config = TestConfiguration.Instance;
        }

        public IWebDriver CreateDriver(BrowserType browserType)
        {
            return browserType switch
            {
                BrowserType.Chrome => CreateChromeDriver(),
                BrowserType.Firefox => CreateFirefoxDriver(),
                BrowserType.Edge => CreateEdgeDriver(),
                _ => throw new ArgumentException($"Unsupported browser type: {browserType}")
            };
        }

        public IWebDriver CreateDriver(string browserName)
        {
            if (Enum.TryParse<BrowserType>(browserName, true, out var browserType))
            {
                return CreateDriver(browserType);
            }
            throw new ArgumentException($"Invalid browser name: {browserName}");
        }

        private IWebDriver CreateChromeDriver()
        {
            var options = new ChromeOptions();

            if (_config.HeadlessMode)
            {
                options.AddArgument("--headless");
            }

            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            var driver = new ChromeDriver(options);
            ConfigureDriver(driver);
            return driver;
        }

        private IWebDriver CreateFirefoxDriver()
        {
            var options = new FirefoxOptions();

            if (_config.HeadlessMode)
            {
                options.AddArgument("--headless");
            }

            var driver = new FirefoxDriver(options);
            ConfigureDriver(driver);
            return driver;
        }

        private IWebDriver CreateEdgeDriver()
        {
            var options = new EdgeOptions();

            if (_config.HeadlessMode)
            {
                options.AddArgument("--headless");
            }

            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");

            var driver = new EdgeDriver(options);
            ConfigureDriver(driver);
            return driver;
        }

        private void ConfigureDriver(IWebDriver driver)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_config.ImplicitWaitSeconds);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(_config.PageLoadTimeoutSeconds);
            driver.Manage().Window.Maximize();
        }
    }
}
