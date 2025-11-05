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
    /// PERFORMANCE OPTIMIZED: Browser instance reused across tests
    /// </summary>
    [TestFixture]
    [Category("Smoke")]
    [Parallelizable(ParallelScope.Self)]
    public class AngularHomePageNUnitTests
    {
        private static SeleniumHooks? _seleniumHooks;
        private static AngularHomePage? _angularHomePage;
        private static TestConfiguration? _config;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Initialize configuration from TestContext parameters (from .runsettings file)
            _config = TestConfiguration.Instance;
            _config.InitializeFromTestContext(TestContext.Parameters);

            // Initialize browser ONCE for all tests using browser from RunSettings
            _seleniumHooks = new SeleniumHooks();
            var browserType = Enum.Parse<BrowserType>(_config.Browser, true);
            _seleniumHooks.InitializeDriver(browserType);
            _angularHomePage = new AngularHomePage(_seleniumHooks);

            TestContext.WriteLine($"Browser initialized once for all tests in fixture: {browserType}");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _seleniumHooks?.QuitDriver();
            TestContext.WriteLine("Browser closed after all tests completed");
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
            // Clear cookies for test isolation
            try
            {
                _seleniumHooks?.GetDriver().Manage().Cookies.DeleteAllCookies();
            }
            catch { }
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
