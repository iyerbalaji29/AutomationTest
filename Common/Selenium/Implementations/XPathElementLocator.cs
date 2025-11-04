using Common.Selenium.Interfaces;
using OpenQA.Selenium;

namespace Common.Selenium.Implementations
{
    /// <summary>
    /// XPath-based element locator implementation
    /// Follows Single Responsibility Principle
    /// </summary>
    public class XPathElementLocator : IElementLocator
    {
        public By GetLocator(string locatorValue)
        {
            if (string.IsNullOrWhiteSpace(locatorValue))
                throw new ArgumentException("Locator value cannot be null or empty", nameof(locatorValue));

            return By.XPath(locatorValue);
        }

        public IWebElement FindElement(IWebDriver driver, string locatorValue)
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));

            var by = GetLocator(locatorValue);
            return driver.FindElement(by);
        }

        public IList<IWebElement> FindElements(IWebDriver driver, string locatorValue)
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));

            var by = GetLocator(locatorValue);
            return driver.FindElements(by);
        }
    }
}
