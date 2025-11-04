using Microsoft.Extensions.Configuration;

namespace Common.Configuration
{
    /// <summary>
    /// Central configuration manager following Single Responsibility Principle
    /// </summary>
    public class TestConfiguration
    {
        private static TestConfiguration? _instance;
        private static readonly object _lock = new object();
        private readonly IConfiguration _configuration;

        private TestConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
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

        public string BaseUrl => _configuration["TestSettings:BaseUrl"] ?? "https://angular.io";
        public string Browser => _configuration["TestSettings:Browser"] ?? "Chrome";
        public int ImplicitWaitSeconds => int.Parse(_configuration["TestSettings:ImplicitWaitSeconds"] ?? "10");
        public int ExplicitWaitSeconds => int.Parse(_configuration["TestSettings:ExplicitWaitSeconds"] ?? "30");
        public int PageLoadTimeoutSeconds => int.Parse(_configuration["TestSettings:PageLoadTimeoutSeconds"] ?? "60");
        public bool HeadlessMode => bool.Parse(_configuration["TestSettings:HeadlessMode"] ?? "false");
        public bool ScreenshotOnFailure => bool.Parse(_configuration["TestSettings:ScreenshotOnFailure"] ?? "true");
        public string ScreenshotPath => _configuration["TestSettings:ScreenshotPath"] ?? "Screenshots";
        public string ConnectionString => _configuration["DatabaseSettings:ConnectionString"] ?? string.Empty;
        public string LogLevel => _configuration["Logging:LogLevel"] ?? "Information";
        public string LogPath => _configuration["Logging:LogPath"] ?? "Logs";

        public string GetValue(string key)
        {
            return _configuration[key] ?? string.Empty;
        }
    }
}
