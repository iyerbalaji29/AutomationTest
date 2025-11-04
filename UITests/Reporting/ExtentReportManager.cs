using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Common.Configuration;

namespace UITests.Reporting
{
    /// <summary>
    /// Manages ExtentReports for test execution reporting
    /// Provides HTML reports with detailed test results
    /// </summary>
    public class ExtentReportManager
    {
        private static ExtentReports? _extent;
        private static readonly object _lock = new object();
        private static string _reportPath = string.Empty;

        /// <summary>
        /// Gets the ExtentReports instance (Singleton pattern)
        /// </summary>
        public static ExtentReports GetExtentReports()
        {
            if (_extent == null)
            {
                lock (_lock)
                {
                    if (_extent == null)
                    {
                        InitializeReport();
                    }
                }
            }
            return _extent!;
        }

        /// <summary>
        /// Initializes the ExtentReports with configuration
        /// </summary>
        private static void InitializeReport()
        {
            var reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestReports");
            Directory.CreateDirectory(reportDirectory);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _reportPath = Path.Combine(reportDirectory, $"TestReport_{timestamp}.html");

            var htmlReporter = new ExtentSparkReporter(_reportPath);

            // Configure the HTML reporter
            htmlReporter.Config.DocumentTitle = "Automation Test Report";
            htmlReporter.Config.ReportName = "Angular SPA Test Execution Report";
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Dark;
            htmlReporter.Config.Encoding = "utf-8";

            _extent = new ExtentReports();
            _extent.AttachReporter(htmlReporter);

            // Add system/environment information
            var config = TestConfiguration.Instance;
            _extent.AddSystemInfo("Application URL", config.BaseUrl);
            _extent.AddSystemInfo("Browser", config.Browser);
            _extent.AddSystemInfo("Environment", Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Local");
            _extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
            _extent.AddSystemInfo("User", Environment.UserName);
            _extent.AddSystemInfo(".NET Version", Environment.Version.ToString());
            _extent.AddSystemInfo("Execution Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// Creates a test case in the report
        /// </summary>
        /// <param name="testName">Name of the test</param>
        /// <param name="description">Test description</param>
        /// <returns>ExtentTest instance</returns>
        public static ExtentTest CreateTest(string testName, string description = "")
        {
            return GetExtentReports().CreateTest(testName, description);
        }

        /// <summary>
        /// Creates a test case with category tags
        /// </summary>
        /// <param name="testName">Name of the test</param>
        /// <param name="description">Test description</param>
        /// <param name="categories">Category tags</param>
        /// <returns>ExtentTest instance</returns>
        public static ExtentTest CreateTest(string testName, string description, params string[] categories)
        {
            var test = GetExtentReports().CreateTest(testName, description);
            foreach (var category in categories)
            {
                test.AssignCategory(category);
            }
            return test;
        }

        /// <summary>
        /// Logs test step information
        /// </summary>
        public static void LogInfo(ExtentTest test, string message)
        {
            test?.Info(message);
        }

        /// <summary>
        /// Logs test step pass status
        /// </summary>
        public static void LogPass(ExtentTest test, string message)
        {
            test?.Pass(message);
        }

        /// <summary>
        /// Logs test step fail status
        /// </summary>
        public static void LogFail(ExtentTest test, string message)
        {
            test?.Fail(message);
        }

        /// <summary>
        /// Logs test step warning
        /// </summary>
        public static void LogWarning(ExtentTest test, string message)
        {
            test?.Warning(message);
        }

        /// <summary>
        /// Logs test step skip status
        /// </summary>
        public static void LogSkip(ExtentTest test, string message)
        {
            test?.Skip(message);
        }

        /// <summary>
        /// Attaches screenshot to the test report
        /// </summary>
        public static void AttachScreenshot(ExtentTest test, string screenshotPath)
        {
            if (test != null && File.Exists(screenshotPath))
            {
                test.AddScreenCaptureFromPath(screenshotPath);
            }
        }

        /// <summary>
        /// Logs exception details to the report
        /// </summary>
        public static void LogException(ExtentTest test, Exception exception)
        {
            if (test != null)
            {
                test.Fail(exception);
            }
        }

        /// <summary>
        /// Flushes the report (writes to disk)
        /// </summary>
        public static void FlushReport()
        {
            _extent?.Flush();
            Console.WriteLine($"Test report generated: {_reportPath}");
        }

        /// <summary>
        /// Gets the current report path
        /// </summary>
        public static string GetReportPath()
        {
            return _reportPath;
        }
    }
}
