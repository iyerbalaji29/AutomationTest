using OpenQA.Selenium;

namespace Common.Selenium.Interfaces
{
    /// <summary>
    /// Interface for common element actions
    /// </summary>
    public interface IElementActions
    {
        void Click(string locator);
        void SendKeys(string locator, string text);
        void Clear(string locator);
        string GetText(string locator);
        string GetAttribute(string locator, string attributeName);
        bool IsDisplayed(string locator);
        bool IsEnabled(string locator);
        bool IsSelected(string locator);
        void WaitForElementVisible(string locator, int timeoutSeconds = 30);
        void WaitForElementClickable(string locator, int timeoutSeconds = 30);
    }
}
