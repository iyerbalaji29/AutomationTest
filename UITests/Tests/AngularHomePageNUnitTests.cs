using Common.Configuration;
using Common.Enums;
using Common.Selenium;
using NUnit.Framework;
using UITests.Pages;

namespace UITests.Tests
{
    /// <summary>
    /// NUnit tests for Angular home page
    /// Demonstrates both ReqNRoll (BDD) and NUnit (traditional) approaches
    /// PERFORMANCE OPTIMIZED: Uses global browser session - browser opens ONCE for entire test run
    /// </summary>
    [TestFixture]
    [Category("Smoke")]
    [Parallelizable(ParallelScope.Self)]
    public class AngularHomePageNUnitTests
    {
        private BrowserSessionManager? _sessionManager;
        private SeleniumHooks? _seleniumHooks;
        private AngularHomePage? _angularHomePage;
        private TestConfiguration? _config;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Get configuration instance
            _config = TestConfiguration.Instance;

            // Get the global browser session (initialized in GlobalSetup)
            _sessionManager = BrowserSessionManager.Instance;
            _seleniumHooks = _sessionManager.GetBrowserSession();

            // Initialize page object
            _angularHomePage = new AngularHomePage(_seleniumHooks);

            TestContext.WriteLine($"Test fixture using global browser session: {_config.Browser}");
        }

        [SetUp]
        public void Setup()
        {
            // Navigate to home page before each test
            _angularHomePage!.NavigateToHomePage(_config!.BaseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            // Clear session (cookies, localStorage, sessionStorage) for test isolation
            _sessionManager?.ClearSession();
        }

        [Test]
        [Description("Verify Angular home page loads successfully")]
        public void VerifyAngularHomePageLoads()
        {
            // Assert - Setup already navigated to home page
            _angularHomePage!.AssertAngularPageLoaded();
            _angularHomePage.AssertLogoIsDisplayed();
            _angularHomePage.AssertPageTitle("Angular");
        }

        [Test]
        [Description("Verify main navigation elements are present")]
        public void VerifyNavigationElements()
        {
            // Assert - Setup already navigated to home page
            _angularHomePage!.AssertLogoIsDisplayed();
            _angularHomePage.AssertGetStartedButtonExists();
            _angularHomePage.AssertDocsLinkIsVisible();
        }

        [Test]
        [Description("Verify page title contains Angular")]
        public void VerifyPageTitle()
        {
            // Assert - Setup already navigated to home page
            _angularHomePage!.AssertPageTitle("Angular");
        }
    }
}
