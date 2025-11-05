using Common.Configuration;
using Common.Enums;
using Common.Selenium;
using Common.Utilities;
using Reqnroll;

namespace UITests.Hooks
{
    /// <summary>
    /// ReqNRoll hooks for test setup and teardown
    /// Uses global browser session - browser opens ONCE for entire test run
    /// </summary>
    [Binding]
    public class TestHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private BrowserSessionManager? _sessionManager;
        private SeleniumHooks? _seleniumHooks;
        private readonly TestConfiguration _config;

        public TestHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _config = TestConfiguration.Instance;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            Logger.Info($"Starting scenario: {_scenarioContext.ScenarioInfo.Title}");

            // Get the global browser session (initialized in GlobalSetup)
            _sessionManager = BrowserSessionManager.Instance;
            _seleniumHooks = _sessionManager.GetBrowserSession();

            // Store in scenario context for access in step definitions
            _scenarioContext["SeleniumHooks"] = _seleniumHooks;

            Logger.Info($"Scenario using global browser session: {_scenarioContext.ScenarioInfo.Title}");
        }

        [AfterScenario]
        public void AfterScenario()
        {
            try
            {
                // Take screenshot on failure if configured
                if (_scenarioContext.TestError != null && _config.ScreenshotOnFailure)
                {
                    var scenarioTitle = _scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
                    var screenshotFileName = $"Failed_{scenarioTitle}";
                    _seleniumHooks?.TakeScreenshot(screenshotFileName);

                    // Store screenshot path for reporting
                    var screenshotPath = Path.Combine(_config.ScreenshotPath, $"{screenshotFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                    _scenarioContext["ScreenshotPath"] = screenshotPath;

                    Logger.Error($"Scenario failed: {_scenarioContext.ScenarioInfo.Title}", _scenarioContext.TestError);
                }

                Logger.Info($"Completed scenario: {_scenarioContext.ScenarioInfo.Title}");
            }
            finally
            {
                // Clear session (cookies, localStorage, sessionStorage) for test isolation
                // Do NOT quit the browser - it's shared across all scenarios
                _sessionManager?.ClearSession();
            }
        }

        private BrowserType GetBrowserFromTags()
        {
            var tags = _scenarioContext.ScenarioInfo.Tags;

            if (tags.Contains("chrome", StringComparer.OrdinalIgnoreCase))
                return BrowserType.Chrome;

            if (tags.Contains("firefox", StringComparer.OrdinalIgnoreCase))
                return BrowserType.Firefox;

            if (tags.Contains("edge", StringComparer.OrdinalIgnoreCase))
                return BrowserType.Edge;

            // Default to config setting
            return Enum.Parse<BrowserType>(_config.Browser, true);
        }
    }
}
