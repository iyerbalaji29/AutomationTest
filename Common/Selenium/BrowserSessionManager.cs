using Common.Configuration;
using Common.Enums;
using OpenQA.Selenium;

namespace Common.Selenium
{
    /// <summary>
    /// Global Browser Session Manager - Singleton pattern
    /// Manages a single browser instance across the entire test run
    /// Provides session/cookie clearing between tests for isolation
    /// </summary>
    public sealed class BrowserSessionManager
    {
        private static readonly Lazy<BrowserSessionManager> _instance =
            new Lazy<BrowserSessionManager>(() => new BrowserSessionManager());

        private SeleniumHooks? _seleniumHooks;
        private bool _isInitialized;
        private readonly object _lock = new object();

        private BrowserSessionManager()
        {
        }

        /// <summary>
        /// Get the singleton instance
        /// </summary>
        public static BrowserSessionManager Instance => _instance.Value;

        /// <summary>
        /// Initialize the browser session once for the entire test run
        /// Thread-safe initialization
        /// </summary>
        public SeleniumHooks InitializeBrowser(BrowserType browserType)
        {
            lock (_lock)
            {
                if (_isInitialized && _seleniumHooks != null)
                {
                    Console.WriteLine($"Browser already initialized with {browserType}. Returning existing instance.");
                    return _seleniumHooks;
                }

                Console.WriteLine($"Initializing browser: {browserType}");
                _seleniumHooks = new SeleniumHooks();
                _seleniumHooks.InitializeDriver(browserType);
                _isInitialized = true;

                Console.WriteLine("Browser initialized successfully");
                return _seleniumHooks;
            }
        }

        /// <summary>
        /// Initialize the browser session from configuration
        /// </summary>
        public SeleniumHooks InitializeBrowser()
        {
            var config = TestConfiguration.Instance;
            var browserType = Enum.Parse<BrowserType>(config.Browser, true);
            return InitializeBrowser(browserType);
        }

        /// <summary>
        /// Get the current browser session
        /// </summary>
        public SeleniumHooks GetBrowserSession()
        {
            if (!_isInitialized || _seleniumHooks == null)
            {
                throw new InvalidOperationException(
                    "Browser session not initialized. Call InitializeBrowser first.");
            }

            return _seleniumHooks;
        }

        /// <summary>
        /// Clear session between tests - clears cookies, local storage, session storage
        /// This maintains test isolation without restarting the browser
        /// </summary>
        public void ClearSession()
        {
            if (!_isInitialized || _seleniumHooks == null)
            {
                return;
            }

            try
            {
                Console.WriteLine("Clearing browser session (cookies, storage)...");

                var driver = _seleniumHooks.GetDriver();

                // Clear cookies
                driver.Manage().Cookies.DeleteAllCookies();

                // Clear local storage
                try
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("localStorage.clear();");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not clear localStorage: {ex.Message}");
                }

                // Clear session storage
                try
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("sessionStorage.clear();");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not clear sessionStorage: {ex.Message}");
                }

                Console.WriteLine("Browser session cleared successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing browser session: {ex.Message}");
            }
        }

        /// <summary>
        /// Navigate to base URL - useful for resetting to home page between tests
        /// </summary>
        public void NavigateToBaseUrl()
        {
            if (!_isInitialized || _seleniumHooks == null)
            {
                throw new InvalidOperationException("Browser session not initialized.");
            }

            var config = TestConfiguration.Instance;
            _seleniumHooks.NavigateTo(config.BaseUrl);
        }

        /// <summary>
        /// Clear session and navigate to base URL
        /// </summary>
        public void ResetSession()
        {
            ClearSession();
            NavigateToBaseUrl();
        }

        /// <summary>
        /// Quit the browser - should only be called at the end of the entire test run
        /// </summary>
        public void QuitBrowser()
        {
            lock (_lock)
            {
                if (_isInitialized && _seleniumHooks != null)
                {
                    Console.WriteLine("Quitting browser...");
                    _seleniumHooks.QuitDriver();
                    _seleniumHooks = null;
                    _isInitialized = false;
                    Console.WriteLine("Browser quit successfully");
                }
            }
        }

        /// <summary>
        /// Check if browser is initialized
        /// </summary>
        public bool IsInitialized => _isInitialized;
    }
}
