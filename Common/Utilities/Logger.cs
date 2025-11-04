using Serilog;
using Common.Configuration;

namespace Common.Utilities
{
    /// <summary>
    /// Logging utility using Serilog
    /// Follows Single Responsibility Principle
    /// </summary>
    public static class Logger
    {
        private static ILogger? _logger;

        static Logger()
        {
            InitializeLogger();
        }

        private static void InitializeLogger()
        {
            var config = TestConfiguration.Instance;
            var logPath = Path.Combine(config.LogPath, $"TestLog_{DateTime.Now:yyyyMMdd}.txt");

            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public static void Info(string message)
        {
            _logger?.Information(message);
        }

        public static void Debug(string message)
        {
            _logger?.Debug(message);
        }

        public static void Warning(string message)
        {
            _logger?.Warning(message);
        }

        public static void Error(string message, Exception? ex = null)
        {
            if (ex != null)
                _logger?.Error(ex, message);
            else
                _logger?.Error(message);
        }

        public static void Fatal(string message, Exception? ex = null)
        {
            if (ex != null)
                _logger?.Fatal(ex, message);
            else
                _logger?.Fatal(message);
        }
    }
}
