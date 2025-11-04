using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Common.Extensions
{
    /// <summary>
    /// Extension methods for IWebElement
    /// </summary>
    public static class WebElementExtensions
    {
        public static void ClickWithJavaScript(this IWebElement element, IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
        }

        public static void SendKeysSlowly(this IWebElement element, string text, int delayMilliseconds = 100)
        {
            foreach (var character in text)
            {
                element.SendKeys(character.ToString());
                Thread.Sleep(delayMilliseconds);
            }
        }

        public static void HoverOver(this IWebElement element, IWebDriver driver)
        {
            var actions = new Actions(driver);
            actions.MoveToElement(element).Perform();
        }

        public static void DoubleClick(this IWebElement element, IWebDriver driver)
        {
            var actions = new Actions(driver);
            actions.DoubleClick(element).Perform();
        }

        public static void RightClick(this IWebElement element, IWebDriver driver)
        {
            var actions = new Actions(driver);
            actions.ContextClick(element).Perform();
        }

        public static bool HasClass(this IWebElement element, string className)
        {
            var classes = element.GetAttribute("class");
            return classes?.Contains(className) ?? false;
        }

        public static void WaitUntilVisible(this IWebElement element, int timeoutSeconds = 30)
        {
            var endTime = DateTime.Now.AddSeconds(timeoutSeconds);
            while (DateTime.Now < endTime)
            {
                if (element.Displayed)
                    return;
                Thread.Sleep(500);
            }
            throw new TimeoutException($"Element was not visible within {timeoutSeconds} seconds");
        }
    }
}
