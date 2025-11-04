using Common.Enums;
using OpenQA.Selenium;

namespace Common.Selenium.Interfaces
{
    /// <summary>
    /// Factory interface for creating WebDriver instances
    /// Follows Factory Pattern and Open/Closed Principle
    /// </summary>
    public interface IWebDriverFactory
    {
        IWebDriver CreateDriver(BrowserType browserType);
        IWebDriver CreateDriver(string browserName);
    }
}
