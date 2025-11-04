using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Common.Extensions
{
    /// <summary>
    /// Extension methods for IWebDriver
    /// Provides additional utility methods
    /// </summary>
    public static class WebDriverExtensions
    {
        public static void WaitForCondition(this IWebDriver driver, Func<IWebDriver, bool> condition, int timeoutSeconds = 30)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            wait.Until(condition);
        }

        public static void ScrollToElement(this IWebDriver driver, IWebElement element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        public static void ScrollToBottom(this IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        }

        public static void ScrollToTop(this IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 0);");
        }

        public static void HighlightElement(this IWebDriver driver, IWebElement element)
        {
            var jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript("arguments[0].style.border='3px solid red'", element);
        }

        public static void SwitchToFrame(this IWebDriver driver, string frameNameOrId)
        {
            driver.SwitchTo().Frame(frameNameOrId);
        }

        public static void SwitchToDefaultContent(this IWebDriver driver)
        {
            driver.SwitchTo().DefaultContent();
        }

        public static void AcceptAlert(this IWebDriver driver)
        {
            driver.SwitchTo().Alert().Accept();
        }

        public static void DismissAlert(this IWebDriver driver)
        {
            driver.SwitchTo().Alert().Dismiss();
        }

        public static string GetAlertText(this IWebDriver driver)
        {
            return driver.SwitchTo().Alert().Text;
        }
    }
}
