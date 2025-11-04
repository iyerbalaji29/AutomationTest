using Common.Selenium;
using Common.Utilities;

namespace Common.PageObjects
{
    /// <summary>
    /// Base Page class that inherits from AbstractPage
    /// Contains common functions for all page objects
    /// Includes Angular-specific functionality
    /// </summary>
    public abstract class BasePage : AbstractPage
    {
        protected BasePage(SeleniumHooks seleniumHooks) : base(seleniumHooks)
        {
        }

        /// <summary>
        /// Navigate to a specific URL
        /// </summary>
        public virtual void NavigateTo(string url)
        {
            Logger.Info($"Navigating to URL: {url}");
            SeleniumHooks.NavigateTo(url);
            WaitForPageLoad();
        }

        /// <summary>
        /// Wait for Angular page to load completely
        /// </summary>
        public virtual void WaitForAngularLoad()
        {
            try
            {
                Logger.Info("Waiting for Angular page to load...");
                SeleniumHooks.WaitForAngular();
                Logger.Info("Angular page loaded successfully");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to wait for Angular page load", ex);
                throw;
            }
        }

        /// <summary>
        /// Verify if Angular page is loaded successfully
        /// </summary>
        public virtual bool IsAngularPageLoadedSuccessfully()
        {
            try
            {
                Logger.Info("Checking if Angular page is loaded...");
                var isLoaded = SeleniumHooks.IsAngularPageLoaded();
                Logger.Info($"Angular page loaded status: {isLoaded}");
                return isLoaded;
            }
            catch (Exception ex)
            {
                Logger.Error("Error checking Angular page load status", ex);
                return false;
            }
        }

        /// <summary>
        /// Click element using XPath
        /// </summary>
        protected virtual void ClickElement(string xpath)
        {
            Logger.Info($"Clicking element: {xpath}");
            SeleniumHooks.Click(xpath);
        }

        /// <summary>
        /// Enter text into element using XPath
        /// </summary>
        protected virtual void EnterText(string xpath, string text)
        {
            Logger.Info($"Entering text into element: {xpath}");
            SeleniumHooks.SendKeys(xpath, text);
        }

        /// <summary>
        /// Get text from element using XPath
        /// </summary>
        protected virtual string GetElementText(string xpath)
        {
            Logger.Info($"Getting text from element: {xpath}");
            return SeleniumHooks.GetText(xpath);
        }

        /// <summary>
        /// Check if element is displayed using XPath
        /// </summary>
        protected virtual bool IsElementDisplayed(string xpath)
        {
            Logger.Info($"Checking if element is displayed: {xpath}");
            return SeleniumHooks.IsDisplayed(xpath);
        }

        /// <summary>
        /// Wait for element to be visible using XPath
        /// </summary>
        protected virtual void WaitForElement(string xpath, int timeoutSeconds = 30)
        {
            Logger.Info($"Waiting for element to be visible: {xpath}");
            SeleniumHooks.WaitForElementVisible(xpath, timeoutSeconds);
        }

        /// <summary>
        /// Take screenshot with custom name
        /// </summary>
        protected virtual void TakeScreenshot(string fileName)
        {
            Logger.Info($"Taking screenshot: {fileName}");
            SeleniumHooks.TakeScreenshot(fileName);
        }

        /// <summary>
        /// Execute JavaScript on the page
        /// </summary>
        protected virtual object ExecuteScript(string script, params object[] args)
        {
            Logger.Info($"Executing JavaScript: {script}");
            return SeleniumHooks.ExecuteJavaScript(script, args);
        }

        /// <summary>
        /// Default implementation - can be overridden by specific pages
        /// </summary>
        public override bool IsPageLoaded()
        {
            WaitForPageLoad();
            return !string.IsNullOrEmpty(GetPageTitle());
        }
    }
}
