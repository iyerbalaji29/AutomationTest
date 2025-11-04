using Common.Enums;
using Common.Selenium.Interfaces;
using OpenQA.Selenium;

namespace Common.Selenium.Implementations
{
    /// <summary>
    /// Factory for creating element locators based on locator type
    /// Supports extension for different locator strategies
    /// </summary>
    public class ElementLocatorFactory
    {
        public static By GetBy(string locatorValue, LocatorType locatorType = LocatorType.XPath)
        {
            if (string.IsNullOrWhiteSpace(locatorValue))
                throw new ArgumentException("Locator value cannot be null or empty", nameof(locatorValue));

            return locatorType switch
            {
                LocatorType.XPath => By.XPath(locatorValue),
                LocatorType.Id => By.Id(locatorValue),
                LocatorType.Name => By.Name(locatorValue),
                LocatorType.ClassName => By.ClassName(locatorValue),
                LocatorType.CssSelector => By.CssSelector(locatorValue),
                LocatorType.TagName => By.TagName(locatorValue),
                LocatorType.LinkText => By.LinkText(locatorValue),
                LocatorType.PartialLinkText => By.PartialLinkText(locatorValue),
                _ => throw new ArgumentException($"Unsupported locator type: {locatorType}")
            };
        }

        public static IElementLocator GetLocator(LocatorType locatorType = LocatorType.XPath)
        {
            return locatorType switch
            {
                LocatorType.XPath => new XPathElementLocator(),
                _ => new XPathElementLocator() // Default to XPath as per requirement
            };
        }
    }
}
