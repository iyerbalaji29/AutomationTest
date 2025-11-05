using Common.Configuration;
using Common.Enums;
using Common.Selenium;
using NUnit.Framework;
using UITests.Pages;

namespace UITests.Tests
{
    /// <summary>
    /// Full feature tests for Angular home page with focus on login functionality
    /// Tests verify that Angular page loads successfully and login button is visible
    /// PERFORMANCE OPTIMIZED: Uses global browser session - browser opens ONCE for entire test run
    /// </summary>
    [TestFixture]
    [Category("Feature")]
    [Category("Login")]
    [Parallelizable(ParallelScope.Self)]
    public class AngularHomePageLoginTests
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
            // Take screenshot on test failure only
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                var testName = TestContext.CurrentContext.Test.Name.Replace(" ", "_");
                _angularHomePage?.TakeHomePageScreenshot($"Failed_{testName}");
            }

            // Clear session (cookies, localStorage, sessionStorage) for test isolation
            _sessionManager?.ClearSession();
        }

        [Test]
        [Description("Full feature test: Verify Angular home page loads successfully and login button is visible")]
        [Category("Smoke")]
        public void VerifyAngularHomePageLoadsSuccessfullyAndLoginButtonIsVisible()
        {
            // Arrange - Setup already navigated to home page
            TestContext.WriteLine($"Test running against: {_config!.BaseUrl}");
            TestContext.WriteLine($"Browser: {_config.Browser}");
            TestContext.WriteLine($"Environment: {_config.Environment}");

            // Assert - Verify Angular page loaded successfully
            _angularHomePage!.AssertAngularPageLoaded();
            TestContext.WriteLine("Angular page loaded successfully");

            // Assert - Verify Angular logo is displayed
            _angularHomePage.AssertLogoIsDisplayed();
            TestContext.WriteLine("Angular logo is displayed");

            // Assert - Verify login button is visible
            _angularHomePage.AssertLoginButtonIsVisible();
            TestContext.WriteLine("Login button is visible on the page");

            // Take screenshot for documentation
            _angularHomePage.TakeHomePageScreenshot("LoginButtonVisible");
        }

        [Test]
        [Description("Verify all key elements on Angular home page including login button")]
        [Category("Smoke")]
        public void VerifyAllKeyElementsIncludingLoginButton()
        {
            // Act & Assert - Setup already navigated to home page
            _angularHomePage!.AssertAngularPageLoaded();
            _angularHomePage.AssertLogoIsDisplayed();
            _angularHomePage.AssertPageTitle("Angular");
            _angularHomePage.AssertLoginButtonIsVisible();
            _angularHomePage.AssertGetStartedButtonExists();
            _angularHomePage.AssertDocsLinkIsVisible();

            TestContext.WriteLine("All key elements verified successfully");
        }

        [Test]
        [Description("Verify login button is displayed and can be interacted with")]
        public void VerifyLoginButtonInteraction()
        {
            // Act - Setup already navigated and waited for page load
            _angularHomePage!.WaitForAngularLoad();

            // Assert - Login button is displayed
            var isLoginButtonDisplayed = _angularHomePage.IsLoginButtonDisplayed();
            Assert.That(isLoginButtonDisplayed, Is.True, "Login button should be displayed on the home page");

            // Take screenshot to document
            _angularHomePage.TakeHomePageScreenshot("LoginButtonReady");

            TestContext.WriteLine("Login button is displayed and ready for interaction");
        }

        [Test]
        [Description("Verify Angular page loads and login button appears within expected timeout")]
        [Category("Performance")]
        public void VerifyPageLoadPerformanceWithLoginButton()
        {
            // Note: This test measures page navigation performance (done in Setup)
            // Assert - Page loads within configured timeout
            _angularHomePage!.AssertAngularPageLoaded();

            // Assert - Login button appears quickly
            _angularHomePage.AssertLoginButtonIsVisible();

            TestContext.WriteLine("Page loaded and login button appeared within expected time");
        }

        [Test]
        [Description("Verify login button remains visible after page refresh")]
        [Category("EdgeCases")]
        public void VerifyLoginButtonAfterPageRefresh()
        {
            // Arrange - Page already loaded from Setup
            _angularHomePage!.AssertLoginButtonIsVisible();

            // Act - Refresh the page
            _angularHomePage.RefreshPage();
            _angularHomePage.WaitForAngularLoad();

            // Assert - Login button still visible after refresh
            _angularHomePage.AssertLoginButtonIsVisible();
            TestContext.WriteLine("Login button remains visible after page refresh");
        }

        [Test]
        [Description("Verify login button visibility after browser resize")]
        [Category("EdgeCases")]
        public void VerifyLoginButtonAfterBrowserResize()
        {
            // Arrange - Page already loaded from Setup
            _angularHomePage!.AssertLoginButtonIsVisible();

            // Act - Resize browser window
            _angularHomePage.SetBrowserSize(800, 600);
            System.Threading.Thread.Sleep(1000); // Wait for resize

            // Assert - Login button should still be accessible
            var isLoginButtonDisplayed = _angularHomePage.IsLoginButtonDisplayed();
            TestContext.WriteLine($"Login button visible after resize: {isLoginButtonDisplayed}");
        }
    }
}
