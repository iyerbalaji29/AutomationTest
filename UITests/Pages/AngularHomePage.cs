using Common.PageObjects;
using Common.Selenium;
using Shouldly;

namespace UITests.Pages
{
    /// <summary>
    /// Page Object for Angular.io Home Page
    /// Inherits from BasePage to get all common functionality
    /// </summary>
    public class AngularHomePage : BasePage
    {
        // XPath locators for page elements
        private const string HomePageLogoXPath = "//a[@aria-label='Angular']";
        private const string GetStartedButtonXPath = "//a[contains(text(),'Get Started')]";
        private const string FeaturesButtonXPath = "//a[contains(@href,'features')]";
        private const string DocsLinkXPath = "//a[contains(text(),'Docs')]";
        private const string PageHeadingXPath = "//h1 | //h2";
        private const string SearchButtonXPath = "//button[@aria-label='Search' or contains(@class,'search')]";
        private const string LoginButtonXPath = "//a[contains(text(),'Login') or contains(text(),'Sign in') or @aria-label='Login' or @aria-label='Sign in']";

        public AngularHomePage(SeleniumHooks seleniumHooks) : base(seleniumHooks)
        {
        }

        /// <summary>
        /// Navigate to Angular home page
        /// </summary>
        public void NavigateToHomePage(string url)
        {
            NavigateTo(url);
            WaitForAngularLoad();
        }

        /// <summary>
        /// Verify if Angular home page is loaded
        /// </summary>
        public override bool IsPageLoaded()
        {
            WaitForPageLoad();
            WaitForAngularLoad();
            return IsElementDisplayed(HomePageLogoXPath);
        }

        /// <summary>
        /// Assert that Angular logo is displayed
        /// </summary>
        public void AssertLogoIsDisplayed()
        {
            var isDisplayed = IsElementDisplayed(HomePageLogoXPath);
            isDisplayed.ShouldBeTrue("Angular logo should be displayed on the home page");
        }

        /// <summary>
        /// Assert that page title contains expected text
        /// </summary>
        public void AssertPageTitle(string expectedTitle)
        {
            var actualTitle = GetPageTitle();
            actualTitle.ShouldContain(expectedTitle, $"Page title should contain '{expectedTitle}'");
        }

        /// <summary>
        /// Click on Get Started button
        /// </summary>
        public void ClickGetStarted()
        {
            WaitForElement(GetStartedButtonXPath);
            ClickElement(GetStartedButtonXPath);
        }

        /// <summary>
        /// Verify Get Started button is displayed
        /// </summary>
        public void AssertGetStartedButtonExists()
        {
            var isDisplayed = IsElementDisplayed(GetStartedButtonXPath);
            isDisplayed.ShouldBeTrue("Get Started button should be displayed");
        }

        /// <summary>
        /// Assert that page URL is correct
        /// </summary>
        public void AssertPageUrl(string expectedUrl)
        {
            var actualUrl = GetCurrentUrl();
            actualUrl.ShouldContain(expectedUrl, $"Page URL should contain '{expectedUrl}'");
        }

        /// <summary>
        /// Assert that Docs link is visible
        /// </summary>
        public void AssertDocsLinkIsVisible()
        {
            var isDisplayed = IsElementDisplayed(DocsLinkXPath);
            isDisplayed.ShouldBeTrue("Docs link should be visible");
        }

        /// <summary>
        /// Click on Docs link
        /// </summary>
        public void ClickDocsLink()
        {
            WaitForElement(DocsLinkXPath);
            ClickElement(DocsLinkXPath);
            WaitForAngularLoad();
        }

        /// <summary>
        /// Get page heading text
        /// </summary>
        public string GetPageHeading()
        {
            WaitForElement(PageHeadingXPath);
            return GetElementText(PageHeadingXPath);
        }

        /// <summary>
        /// Assert Angular page loaded successfully
        /// </summary>
        public void AssertAngularPageLoaded()
        {
            var isLoaded = IsAngularPageLoadedSuccessfully();
            isLoaded.ShouldBeTrue("Angular page should be loaded successfully");
        }

        /// <summary>
        /// Take screenshot of home page
        /// </summary>
        public void TakeHomePageScreenshot(string fileName)
        {
            TakeScreenshot($"HomePage_{fileName}");
        }

        /// <summary>
        /// Assert that Login button is visible
        /// </summary>
        public void AssertLoginButtonIsVisible()
        {
            var isDisplayed = IsElementDisplayed(LoginButtonXPath);
            isDisplayed.ShouldBeTrue("Login button should be visible on the home page");
        }

        /// <summary>
        /// Click on Login button
        /// </summary>
        public void ClickLoginButton()
        {
            WaitForElement(LoginButtonXPath);
            ClickElement(LoginButtonXPath);
        }

        /// <summary>
        /// Verify if Login button exists on page
        /// </summary>
        public bool IsLoginButtonDisplayed()
        {
            return IsElementDisplayed(LoginButtonXPath);
        }
    }
}
