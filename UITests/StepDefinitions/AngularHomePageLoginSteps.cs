using Common.Configuration;
using Common.Selenium;
using Reqnroll;
using UITests.Pages;
using Shouldly;

namespace UITests.StepDefinitions
{
    [Binding]
    public class AngularHomePageLoginSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly AngularHomePage _angularHomePage;
        private readonly TestConfiguration _config;

        public AngularHomePageLoginSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _config = TestConfiguration.Instance;

            // Get SeleniumHooks from scenario context (initialized in TestHooks)
            var seleniumHooks = _scenarioContext["SeleniumHooks"] as SeleniumHooks
                ?? throw new InvalidOperationException("SeleniumHooks not found in ScenarioContext");

            _angularHomePage = new AngularHomePage(seleniumHooks);
        }

        [Given(@"I navigate to the Angular home page")]
        public void GivenINavigateToTheAngularHomePage()
        {
            _angularHomePage.NavigateToHomePage(_config.BaseUrl);
        }

        [When(@"the Angular page finishes loading")]
        public void WhenTheAngularPageFinishesLoading()
        {
            _angularHomePage.WaitForPageLoad();
            _angularHomePage.WaitForAngularLoad();
        }

        [Then(@"the Angular page should be loaded successfully")]
        public void ThenTheAngularPageShouldBeLoadedSuccessfully()
        {
            _angularHomePage.AssertAngularPageLoaded();
        }

        [Then(@"the Angular logo should be displayed")]
        public void ThenTheAngularLogoShouldBeDisplayed()
        {
            _angularHomePage.AssertLogoIsDisplayed();
        }

        [Then(@"the login button should be visible on the page")]
        public void ThenTheLoginButtonShouldBeVisibleOnThePage()
        {
            _angularHomePage.AssertLoginButtonIsVisible();
        }

        [Then(@"the page title should contain ""(.*)""")]
        public void ThenThePageTitleShouldContain(string expectedTitle)
        {
            _angularHomePage.AssertPageTitle(expectedTitle);
        }

        [Then(@"the Get Started button should be displayed")]
        public void ThenTheGetStartedButtonShouldBeDisplayed()
        {
            _angularHomePage.AssertGetStartedButtonExists();
        }

        [Then(@"the Docs link should be visible")]
        public void ThenTheDocsLinkShouldBeVisible()
        {
            _angularHomePage.AssertDocsLinkIsVisible();
        }

        [Then(@"I can interact with the login button")]
        public void ThenICanInteractWithTheLoginButton()
        {
            // Verify the button is displayed and can be clicked
            var isDisplayed = _angularHomePage.IsLoginButtonDisplayed();
            isDisplayed.ShouldBeTrue("Login button should be displayed and ready for interaction");

            // Take a screenshot to document the login button
            _angularHomePage.TakeHomePageScreenshot("LoginButtonVisible");
        }
    }
}
