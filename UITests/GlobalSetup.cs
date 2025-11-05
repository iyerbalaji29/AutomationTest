using Common.Configuration;
using Common.Enums;
using Common.Selenium;
using NUnit.Framework;

namespace UITests
{
    /// <summary>
    /// Global Test Setup - runs ONCE for the entire test assembly
    /// Initializes browser session at the start and quits at the end
    /// Individual tests use session clearing for isolation
    /// </summary>
    [SetUpFixture]
    public class GlobalSetup
    {
        private BrowserSessionManager? _sessionManager;

        /// <summary>
        /// Runs ONCE before any tests in the assembly
        /// Initializes the browser session from RunSettings configuration
        /// </summary>
        [OneTimeSetUp]
        public void GlobalOneTimeSetup()
        {
            Console.WriteLine("=== GLOBAL SETUP: Starting test run ===");

            // Initialize configuration from TestContext
            var config = TestConfiguration.Instance;
            config.InitializeFromTestContext(TestContext.Parameters);

            Console.WriteLine($"Configuration loaded:");
            Console.WriteLine($"  - Browser: {config.Browser}");
            Console.WriteLine($"  - BaseUrl: {config.BaseUrl}");
            Console.WriteLine($"  - Environment: {config.Environment}");
            Console.WriteLine($"  - HeadlessMode: {config.HeadlessMode}");

            // Initialize browser session (single instance for entire run)
            _sessionManager = BrowserSessionManager.Instance;
            var browserType = Enum.Parse<BrowserType>(config.Browser, true);

            Console.WriteLine($"Initializing browser: {browserType}");
            _sessionManager.InitializeBrowser(browserType);

            Console.WriteLine("=== GLOBAL SETUP: Browser initialized successfully ===");
        }

        /// <summary>
        /// Runs ONCE after all tests in the assembly complete
        /// Quits the browser session
        /// </summary>
        [OneTimeTearDown]
        public void GlobalOneTimeTearDown()
        {
            Console.WriteLine("=== GLOBAL TEARDOWN: Finishing test run ===");

            // Quit browser
            if (_sessionManager != null)
            {
                Console.WriteLine("Quitting browser...");
                _sessionManager.QuitBrowser();
            }

            Console.WriteLine("=== GLOBAL TEARDOWN: Complete ===");
        }
    }
}
