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
    /// </summary>
    [TestFixture]
    [Category("Smoke")]
    public class AngularHomePageNUnitTests
    {
        private SeleniumHooks? _seleniumHooks;
        private AngularHomePage? _angularHomePage;
        private TestConfiguration? _config;

        [SetUp]
        public void Setup()
        {
            _config = TestConfiguration.Instance;
            _seleniumHooks = new SeleniumHooks();
            _seleniumHooks.InitializeDriver(BrowserType.Chrome);
            _angularHomePage = new AngularHomePage(_seleniumHooks);
        }

        [TearDown]
        public void TearDown()
        {
            _seleniumHooks?.QuitDriver();
        }

        [Test]
        [Description("Verify Angular home page loads successfully")]
        public void VerifyAngularHomePageLoads()
        {
            // Arrange & Act
            _angularHomePage!.NavigateToHomePage(_config!.BaseUrl);

            // Assert
            _angularHomePage.AssertAngularPageLoaded();
            _angularHomePage.AssertLogoIsDisplayed();
            _angularHomePage.AssertPageTitle("Angular");
        }

        [Test]
        [Description("Verify main navigation elements are present")]
        public void VerifyNavigationElements()
        {
            // Arrange & Act
            _angularHomePage!.NavigateToHomePage(_config!.BaseUrl);

            // Assert
            _angularHomePage.AssertLogoIsDisplayed();
            _angularHomePage.AssertGetStartedButtonExists();
            _angularHomePage.AssertDocsLinkIsVisible();
        }

        [Test]
        [Category("Chrome")]
        [Description("Verify page title contains Angular")]
        public void VerifyPageTitle()
        {
            // Arrange & Act
            _angularHomePage!.NavigateToHomePage(_config!.BaseUrl);

            // Assert
            _angularHomePage.AssertPageTitle("Angular");
        }
    }

    /// <summary>
    /// Multi-browser tests using NUnit
    /// Demonstrates parameterized tests for cross-browser testing
    /// </summary>
    [TestFixture]
    [Category("CrossBrowser")]
    public class AngularHomePageCrossBrowserTests
    {
        private SeleniumHooks? _seleniumHooks;
        private AngularHomePage? _angularHomePage;
        private TestConfiguration? _config;

        [SetUp]
        public void Setup()
        {
            _config = TestConfiguration.Instance;
        }

        [TearDown]
        public void TearDown()
        {
            _seleniumHooks?.QuitDriver();
        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        [Description("Verify Angular page loads in different browsers")]
        public void VerifyAngularPageInDifferentBrowsers(BrowserType browserType)
        {
            // Arrange
            _seleniumHooks = new SeleniumHooks();
            _seleniumHooks.InitializeDriver(browserType);
            _angularHomePage = new AngularHomePage(_seleniumHooks);

            // Act
            _angularHomePage.NavigateToHomePage(_config!.BaseUrl);

            // Assert
            _angularHomePage.AssertAngularPageLoaded();
            _angularHomePage.AssertLogoIsDisplayed();
        }
    }
}
