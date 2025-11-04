using Common.Configuration;
using Common.Selenium;
using Reqnroll;
using UITests.Pages;

namespace UITests.StepDefinitions
{
    /// <summary>
    /// Step definitions for Angular home page tests
    /// Implements BDD scenarios using ReqNRoll
    /// </summary>
    [Binding]
    public class AngularHomePageSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly SeleniumHooks _seleniumHooks;
        private readonly AngularHomePage _angularHomePage;
        private readonly TestConfiguration _config;

        public AngularHomePageSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _seleniumHooks = (SeleniumHooks)_scenarioContext["SeleniumHooks"];
            _angularHomePage = new AngularHomePage(_seleniumHooks);
            _config = TestConfiguration.Instance;
        }

        [Given(@"I navigate to the Angular home page")]
        public void GivenINavigateToTheAngularHomePage()
        {
            _angularHomePage.NavigateToHomePage(_config.BaseUrl);
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

        [When(@"I click on the Docs link")]
        public void WhenIClickOnTheDocsLink()
        {
            _angularHomePage.ClickDocsLink();
        }

        [Then(@"the page URL should contain ""(.*)""")]
        public void ThenThePageUrlShouldContain(string expectedUrl)
        {
            _angularHomePage.AssertPageUrl(expectedUrl);
        }

        [Then(@"I take a screenshot of the home page")]
        public void ThenITakeAScreenshotOfTheHomePage()
        {
            var scenarioTitle = _scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
            _angularHomePage.TakeHomePageScreenshot(scenarioTitle);
        }
    }
}
