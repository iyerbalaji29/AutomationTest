namespace Common.Configuration
{
    /// <summary>
    /// Central configuration manager following Single Responsibility Principle
    /// Now reads from .runsettings files via TestContext
    /// </summary>
    public class TestConfiguration
    {
        private static TestConfiguration? _instance;
        private static readonly object _lock = new object();
        private readonly Dictionary<string, string> _parameters;

        private TestConfiguration()
        {
            _parameters = new Dictionary<string, string>();
        }

        public static TestConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new TestConfiguration();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initialize configuration from TestContext (called by test hooks)
        /// </summary>
        public void InitializeFromTestContext(IDictionary<string, object?>? testParameters)
        {
            if (testParameters == null) return;

            _parameters.Clear();
            foreach (var param in testParameters)
            {
                if (param.Value != null)
                {
                    _parameters[param.Key] = param.Value.ToString() ?? string.Empty;
                }
            }
        }

        private string GetParameter(string key, string defaultValue = "")
        {
            return _parameters.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public string BaseUrl => GetParameter("BaseUrl", "https://angular.io");
        public string Browser => GetParameter("Browser", "Chrome");
        public int ImplicitWaitSeconds => int.Parse(GetParameter("ImplicitWaitSeconds", "10"));
        public int ExplicitWaitSeconds => int.Parse(GetParameter("ExplicitWaitSeconds", "30"));
        public int PageLoadTimeoutSeconds => int.Parse(GetParameter("PageLoadTimeoutSeconds", "60"));
        public bool HeadlessMode => bool.Parse(GetParameter("HeadlessMode", "false"));
        public bool ScreenshotOnFailure => bool.Parse(GetParameter("ScreenshotOnFailure", "true"));
        public string ScreenshotPath => GetParameter("ScreenshotPath", "Screenshots");
        public string ConnectionString => GetParameter("ConnectionString", string.Empty);
        public string LogLevel => GetParameter("LogLevel", "Information");
        public string LogPath => GetParameter("LogPath", "Logs");
        public string Environment => GetParameter("Environment", "Dev");

        public string GetValue(string key)
        {
            return GetParameter(key, string.Empty);
        }
    }
}
