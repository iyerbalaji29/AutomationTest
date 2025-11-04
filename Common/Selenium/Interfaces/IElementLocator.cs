using OpenQA.Selenium;

namespace Common.Selenium.Interfaces
{
    /// <summary>
    /// Interface for element location strategies
    /// Follows Interface Segregation Principle
    /// </summary>
    public interface IElementLocator
    {
        By GetLocator(string locatorValue);
        IWebElement FindElement(IWebDriver driver, string locatorValue);
        IList<IWebElement> FindElements(IWebDriver driver, string locatorValue);
    }
}
