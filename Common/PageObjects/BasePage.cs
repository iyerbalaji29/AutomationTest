using Common.Selenium;
using Common.Utilities;
using OpenQA.Selenium;

namespace Common.PageObjects
{
    /// <summary>
    /// Base Page class that inherits from AbstractPage
    /// Contains common functions for all page objects
    /// Includes Angular-specific functionality
    /// Uses Template Method pattern - child pages can override behavior
    /// </summary>
    public abstract class BasePage : AbstractPage
    {
        protected BasePage(SeleniumHooks seleniumHooks) : base(seleniumHooks)
        {
        }

        #region Navigation Methods - Can be overridden by child pages

        /// <summary>
        /// Navigate to a specific URL
        /// Can be overridden to add page-specific navigation logic
        /// </summary>
        public virtual void NavigateTo(string url)
        {
            Logger.Info($"Navigating to URL: {url}");
            SeleniumHooks.NavigateTo(url);
            WaitForPageLoad();
            OnPageNavigated(url);
        }

        /// <summary>
        /// Hook method called after page navigation completes
        /// Override to add custom post-navigation logic
        /// </summary>
        protected virtual void OnPageNavigated(string url)
        {
            // Default: no action
            // Child pages can override to add custom behavior
        }

        #endregion

        #region Angular-Specific Methods - Can be overridden by child pages

        /// <summary>
        /// Wait for Angular page to load completely
        /// Can be overridden for custom Angular wait logic
        /// </summary>
        public virtual void WaitForAngularLoad()
        {
            try
            {
                Logger.Info("Waiting for Angular page to load...");
                SeleniumHooks.WaitForAngular();
                Logger.Info("Angular page loaded successfully");
                OnAngularLoadComplete();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to wait for Angular page load", ex);
                throw;
            }
        }

        /// <summary>
        /// Hook method called after Angular load completes
        /// Override to add custom post-load logic
        /// </summary>
        protected virtual void OnAngularLoadComplete()
        {
            // Default: no action
            // Child pages can override to add custom behavior
        }

        /// <summary>
        /// Verify if Angular page is loaded successfully
        /// Can be overridden for custom verification logic
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

        #endregion

        #region Element Interaction Methods - Can be overridden by child pages

        /// <summary>
        /// Click element using XPath
        /// Can be overridden to add custom click logic (logging, retries, etc.)
        /// </summary>
        protected virtual void ClickElement(string xpath)
        {
            Logger.Info($"Clicking element: {xpath}");
            BeforeElementClick(xpath);
            SeleniumHooks.Click(xpath);
            AfterElementClick(xpath);
        }

        /// <summary>
        /// Hook method called before clicking an element
        /// </summary>
        protected virtual void BeforeElementClick(string xpath)
        {
            // Default: no action
        }

        /// <summary>
        /// Hook method called after clicking an element
        /// </summary>
        protected virtual void AfterElementClick(string xpath)
        {
            // Default: no action
        }

        /// <summary>
        /// Enter text into element using XPath
        /// Can be overridden to add custom text entry logic
        /// </summary>
        protected virtual void EnterText(string xpath, string text)
        {
            Logger.Info($"Entering text into element: {xpath}");
            SeleniumHooks.SendKeys(xpath, text);
        }

        /// <summary>
        /// Get text from element using XPath
        /// Can be overridden to add custom text retrieval logic
        /// </summary>
        protected virtual string GetElementText(string xpath)
        {
            Logger.Info($"Getting text from element: {xpath}");
            return SeleniumHooks.GetText(xpath);
        }

        /// <summary>
        /// Check if element is displayed using XPath
        /// Can be overridden to add custom visibility check logic
        /// </summary>
        protected virtual bool IsElementDisplayed(string xpath)
        {
            Logger.Info($"Checking if element is displayed: {xpath}");
            return SeleniumHooks.IsDisplayed(xpath);
        }

        /// <summary>
        /// Wait for element to be visible using XPath
        /// Can be overridden to add custom wait logic
        /// </summary>
        protected virtual void WaitForElement(string xpath, int timeoutSeconds = 30)
        {
            Logger.Info($"Waiting for element to be visible: {xpath}");
            SeleniumHooks.WaitForElementVisible(xpath, timeoutSeconds);
        }

        /// <summary>
        /// Find element using XPath
        /// Can be overridden for custom element location logic
        /// </summary>
        protected virtual IWebElement FindElement(string xpath)
        {
            Logger.Info($"Finding element: {xpath}");
            return SeleniumHooks.FindElement(xpath);
        }

        /// <summary>
        /// Find multiple elements using XPath
        /// Can be overridden for custom elements location logic
        /// </summary>
        protected virtual IList<IWebElement> FindElements(string xpath)
        {
            Logger.Info($"Finding elements: {xpath}");
            return SeleniumHooks.FindElements(xpath);
        }

        #endregion

        #region Screenshot and Utility Methods - Can be overridden by child pages

        /// <summary>
        /// Take screenshot with custom name
        /// Can be overridden to add custom screenshot logic
        /// </summary>
        protected virtual void TakeScreenshot(string fileName)
        {
            Logger.Info($"Taking screenshot: {fileName}");
            SeleniumHooks.TakeScreenshot(fileName);
        }

        /// <summary>
        /// Execute JavaScript on the page
        /// Can be overridden to add custom script execution logic
        /// </summary>
        protected virtual object ExecuteScript(string script, params object[] args)
        {
            Logger.Info($"Executing JavaScript: {script}");
            return SeleniumHooks.ExecuteJavaScript(script, args);
        }

        /// <summary>
        /// Set browser window size
        /// Can be overridden for custom browser sizing logic
        /// </summary>
        public virtual void SetBrowserSize(int width, int height)
        {
            Logger.Info($"Setting browser size to {width}x{height}");
            Driver.Manage().Window.Size = new System.Drawing.Size(width, height);
        }

        #endregion

        #region Page Load Verification - Can be overridden by child pages

        /// <summary>
        /// Default implementation - can be overridden by specific pages
        /// Override to provide page-specific load verification
        /// </summary>
        public override bool IsPageLoaded()
        {
            WaitForPageLoad();
            return !string.IsNullOrEmpty(GetPageTitle());
        }

        #endregion
    }
}
