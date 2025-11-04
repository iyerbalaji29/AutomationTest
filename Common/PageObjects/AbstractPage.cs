using Common.Selenium;
using OpenQA.Selenium;

namespace Common.PageObjects
{
    /// <summary>
    /// Abstract Page class with common page functionalities
    /// Follows Template Method pattern
    /// </summary>
    public abstract class AbstractPage
    {
        protected readonly SeleniumHooks SeleniumHooks;
        protected IWebDriver Driver => SeleniumHooks.GetDriver();

        protected AbstractPage(SeleniumHooks seleniumHooks)
        {
            SeleniumHooks = seleniumHooks ?? throw new ArgumentNullException(nameof(seleniumHooks));
        }

        /// <summary>
        /// Verify if the page is loaded successfully
        /// Must be implemented by derived classes
        /// </summary>
        public abstract bool IsPageLoaded();

        /// <summary>
        /// Get the page title
        /// </summary>
        public virtual string GetPageTitle()
        {
            return Driver.Title;
        }

        /// <summary>
        /// Get the current URL
        /// </summary>
        public virtual string GetCurrentUrl()
        {
            return Driver.Url;
        }

        /// <summary>
        /// Wait for page to be fully loaded
        /// </summary>
        public virtual void WaitForPageLoad()
        {
            SeleniumHooks.WaitForPageLoad();
        }

        /// <summary>
        /// Refresh the current page
        /// </summary>
        public virtual void RefreshPage()
        {
            SeleniumHooks.Refresh();
        }
    }
}
