using AventStack.ExtentReports;
using Common.Utilities;
using Reqnroll;
using UITests.Reporting;

namespace UITests.Hooks
{
    /// <summary>
    /// Hooks for test reporting integration
    /// Manages ExtentReports lifecycle and test result logging
    /// </summary>
    [Binding]
    public class ReportingHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest? _currentTest;

        public ReportingHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            // Initialize the report at the start of test run
            ExtentReportManager.GetExtentReports();
            Logger.Info("Test execution started - Report initialized");
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            // Create test entry in report
            var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
            var tags = _scenarioContext.ScenarioInfo.Tags.ToArray();

            _currentTest = ExtentReportManager.CreateTest(scenarioTitle, "", tags);

            // Store in scenario context
            _scenarioContext["ExtentTest"] = _currentTest;

            // Log scenario start
            ExtentReportManager.LogInfo(_currentTest, $"Starting scenario: {scenarioTitle}");

            // Log tags
            if (tags.Length > 0)
            {
                ExtentReportManager.LogInfo(_currentTest, $"Tags: {string.Join(", ", tags)}");
            }
        }

        [AfterStep]
        public void AfterStep()
        {
            if (_currentTest == null) return;

            var stepType = _scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            var stepText = _scenarioContext.StepContext.StepInfo.Text;

            if (_scenarioContext.TestError == null)
            {
                // Step passed
                ExtentReportManager.LogPass(_currentTest, $"{stepType} {stepText}");
            }
            else
            {
                // Step failed
                ExtentReportManager.LogFail(_currentTest, $"{stepType} {stepText} - FAILED");
            }
        }

        [AfterScenario]
        public void AfterScenario()
        {
            if (_currentTest == null) return;

            var scenarioTitle = _scenarioContext.ScenarioInfo.Title;

            if (_scenarioContext.TestError != null)
            {
                // Scenario failed
                ExtentReportManager.LogFail(_currentTest, $"Scenario failed: {scenarioTitle}");
                ExtentReportManager.LogException(_currentTest, _scenarioContext.TestError);

                // Attach screenshot if available
                var screenshotPath = _scenarioContext.ContainsKey("ScreenshotPath")
                    ? _scenarioContext["ScreenshotPath"].ToString()
                    : null;

                if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
                {
                    ExtentReportManager.AttachScreenshot(_currentTest, screenshotPath);
                }

                Logger.Error($"Scenario failed: {scenarioTitle}", _scenarioContext.TestError);
            }
            else if (_scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.Skipped)
            {
                // Scenario skipped
                ExtentReportManager.LogSkip(_currentTest, $"Scenario skipped: {scenarioTitle}");
            }
            else
            {
                // Scenario passed
                ExtentReportManager.LogPass(_currentTest, $"Scenario passed: {scenarioTitle}");
                Logger.Info($"Scenario passed: {scenarioTitle}");
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            // Flush the report to disk
            ExtentReportManager.FlushReport();
            Logger.Info($"Test execution completed - Report available at: {ExtentReportManager.GetReportPath()}");

            // Generate additional summaries for CI/CD
            try
            {
                var publisher = new TestResultPublisher();
                var summary = new TestResultSummary
                {
                    Timestamp = DateTime.Now,
                    Environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Local"
                };

                // Generate JSON summary
                var jsonPath = Path.Combine("TestResults", $"summary_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                publisher.GenerateJsonSummary(summary, jsonPath);

                // Generate Markdown summary
                var mdPath = Path.Combine("TestResults", $"summary_{DateTime.Now:yyyyMMdd_HHmmss}.md");
                publisher.GenerateMarkdownSummary(summary, mdPath);

                Logger.Info("Test summaries generated for CI/CD pipeline");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to generate test summaries", ex);
            }
        }
    }
}
