# Test Reporting Guide

Comprehensive guide for test reporting, report generation, and analysis in the Automation Test Framework.

## Table of Contents

- [Overview](#overview)
- [Report Types](#report-types)
- [ExtentReports](#extentreports)
- [Azure DevOps Reports](#azure-devops-reports)
- [Custom Reports](#custom-reports)
- [Report Analysis](#report-analysis)
- [Best Practices](#best-practices)

## Overview

The framework generates multiple types of reports to provide comprehensive test execution visibility:

1. **ExtentReports** - Interactive HTML reports
2. **TRX Reports** - Visual Studio test results
3. **Code Coverage** - Test coverage metrics
4. **JSON Summary** - Machine-readable results
5. **Markdown Summary** - Human-readable summary

## Report Types

### 1. ExtentReports (HTML)

**Location**: `UITests/TestReports/TestReport_*.html`

**Features**:
- Interactive dashboard
- Test execution timeline
- Step-by-step logs
- Screenshots on failure
- Filtering and search
- Environment information
- Execution statistics
- Dark/Light theme

**Sample Dashboard**:
```
┌─────────────────────────────────────────┐
│  Automation Test Report                  │
│                                          │
│  Total: 25  Passed: 23  Failed: 2       │
│  Success Rate: 92%                       │
│                                          │
│  ✅ Smoke Tests (15/15)                  │
│  ❌ Regression Tests (8/10)              │
│                                          │
│  [Chrome] [Firefox] [Edge]               │
│  [Smoke] [Regression] [Critical]         │
└─────────────────────────────────────────┘
```

**Opening ExtentReports**:
```bash
# Windows
start UITests\TestReports\TestReport_*.html

# Linux/Mac
xdg-open UITests/TestReports/TestReport_*.html
```

### 2. TRX Reports (Visual Studio Test Results)

**Location**: `UITests/TestResults/*.trx`

**Features**:
- XML format
- Test pass/fail status
- Execution time per test
- Error messages and stack traces
- Azure DevOps integration

**Opening TRX Reports**:
```bash
# Visual Studio
File → Open → TestResults.trx

# Azure DevOps
Automatic integration via pipeline
```

### 3. Code Coverage Reports

**Location**: `UITests/TestResults/**/coverage.cobertura.xml`

**Features**:
- Line coverage percentage
- Branch coverage
- Method coverage
- Class coverage
- Uncovered lines highlighting

**Viewing Coverage**:
```bash
# Install Report Generator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:CoverageReport" "-reporttypes:Html"

# Open report
start CoverageReport\index.html
```

### 4. JSON Summary

**Location**: `UITests/TestResults/summary_*.json`

**Format**:
```json
{
  "Total": 25,
  "Passed": 23,
  "Failed": 2,
  "Skipped": 0,
  "ExecutionTime": "5m 32s",
  "Environment": "Test",
  "Browser": "Chrome",
  "Timestamp": "2025-01-04T10:30:00Z"
}
```

**Usage**:
- API integration
- Dashboard consumption
- Automated analysis
- Trend tracking

### 5. Markdown Summary

**Location**: `UITests/TestResults/summary_*.md`

**Format**:
```markdown
# Test Execution Summary

## Overall Results

| Metric | Count |
|--------|-------|
| Total Tests | 25 |
| Passed | ✅ 23 |
| Failed | ❌ 2 |
| Success Rate | 92% |

## Status
❌ **2 test(s) failed**
```

**Usage**:
- GitHub/Azure DevOps comments
- Email notifications
- Slack messages
- Documentation

## ExtentReports

### Configuration

**Location**: `UITests/Reporting/ExtentReportManager.cs`

**Customization**:
```csharp
// Document title
htmlReporter.Config.DocumentTitle = "My Custom Title";

// Report name
htmlReporter.Config.ReportName = "My Test Report";

// Theme
htmlReporter.Config.Theme = Theme.Dark; // or Theme.Standard

// Encoding
htmlReporter.Config.Encoding = "utf-8";

// Add system information
_extent.AddSystemInfo("Application", "MyApp");
_extent.AddSystemInfo("Version", "2.0.0");
_extent.AddSystemInfo("Sprint", "Sprint 15");
_extent.AddSystemInfo("Team", "QA Team");
```

### Test Logging

**In Step Definitions**:
```csharp
[Binding]
public class MySteps
{
    private readonly ExtentTest _test;

    public MySteps(ScenarioContext scenarioContext)
    {
        _test = (ExtentTest)scenarioContext["ExtentTest"];
    }

    [Given(@"I perform an action")]
    public void GivenIPerformAnAction()
    {
        ExtentReportManager.LogInfo(_test, "Performing action...");

        try
        {
            // Perform action
            ExtentReportManager.LogPass(_test, "Action completed successfully");
        }
        catch (Exception ex)
        {
            ExtentReportManager.LogFail(_test, "Action failed");
            ExtentReportManager.LogException(_test, ex);
            throw;
        }
    }
}
```

### Screenshot Attachment

**Automatic** (on failure):
```csharp
// In TestHooks.cs
if (_scenarioContext.TestError != null)
{
    var screenshotPath = TakeScreenshot();
    _scenarioContext["ScreenshotPath"] = screenshotPath;
}

// In ReportingHooks.cs
var screenshotPath = _scenarioContext["ScreenshotPath"].ToString();
ExtentReportManager.AttachScreenshot(_currentTest, screenshotPath);
```

**Manual**:
```csharp
var screenshotPath = _hooks.TakeScreenshot("custom_screenshot");
ExtentReportManager.AttachScreenshot(_test, screenshotPath);
```

### Categories/Tags

**Adding Categories**:
```csharp
// Single category
_test.AssignCategory("Smoke");

// Multiple categories
_test.AssignCategory("Smoke", "Critical", "Chrome");

// Automatic from ReqNRoll tags
var tags = _scenarioContext.ScenarioInfo.Tags;
foreach (var tag in tags)
{
    _test.AssignCategory(tag);
}
```

**Filtering by Category**:
In the HTML report:
1. Click on category chips
2. Filter tests by category
3. View category-specific statistics

### Report Features

**Dashboard View**:
- Overall pass/fail statistics
- Execution timeline
- Category distribution
- Test duration analysis

**Test View**:
- Test name and description
- Execution status (Pass/Fail/Skip)
- Step-by-step execution
- Screenshots
- Logs
- Execution time

**Category View**:
- Group tests by category
- Category-wise statistics
- Drill down to test details

**Exception View**:
- All failed tests
- Exception messages
- Stack traces
- Screenshots

## Azure DevOps Reports

### Test Results Tab

**Features**:
- Pass/Fail/Skipped counts
- Test duration
- Error messages
- Filter and group tests
- Trend analysis
- Comparison with previous runs

**Accessing**:
1. Open pipeline run
2. Click **Tests** tab
3. View results and analytics

**Filtering Tests**:
```
- By Outcome: Passed, Failed, Skipped
- By Duration: Fast, Slow
- By Test Name: Search and filter
- By Owner: Team member
```

**Test Analytics**:
```
- Pass rate trend
- Average duration
- Flaky tests
- Most failed tests
```

### Code Coverage Tab

**Features**:
- Overall coverage percentage
- Line coverage
- Branch coverage
- File-level coverage
- Method-level coverage

**Accessing**:
1. Open pipeline run
2. Click **Code Coverage** tab
3. View coverage metrics

### Build Artifacts

**Downloading Artifacts**:
1. Open pipeline run
2. Click **Artifacts** button (top right)
3. Download specific artifacts:
   - TestReports
   - Screenshots
   - Logs
   - ConsolidatedReport

**Artifact Contents**:
```
TestReports/
├── TestReport_20250104_103000.html
└── index.html

Screenshots/
├── Failed_LoginTest_20250104_103015.png
└── Failed_CheckoutTest_20250104_103045.png

Logs/
├── TestLog_20250104.txt
└── execution.log

ConsolidatedReport.md
```

## Custom Reports

### Creating Custom Reports

**JSON Report Generator**:
```csharp
public class JsonReportGenerator
{
    public void GenerateReport(List<TestResult> results, string outputPath)
    {
        var report = new
        {
            Summary = new
            {
                Total = results.Count,
                Passed = results.Count(r => r.Status == "Passed"),
                Failed = results.Count(r => r.Status == "Failed"),
                Duration = results.Sum(r => r.Duration)
            },
            Tests = results.Select(r => new
            {
                r.Name,
                r.Status,
                r.Duration,
                r.Browser,
                r.Tags
            })
        };

        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(outputPath, json);
    }
}
```

**CSV Report Generator**:
```csharp
public class CsvReportGenerator
{
    public void GenerateReport(List<TestResult> results, string outputPath)
    {
        var csv = new StringBuilder();
        csv.AppendLine("TestName,Status,Duration,Browser,Tags");

        foreach (var result in results)
        {
            csv.AppendLine($"{result.Name},{result.Status},{result.Duration},{result.Browser},{string.Join(";", result.Tags)}");
        }

        File.WriteAllText(outputPath, csv.ToString());
    }
}
```

### Email Report

**Using SendGrid**:
```csharp
public class EmailReporter
{
    public async Task SendReportEmail(TestResultSummary summary)
    {
        var client = new SendGridClient(apiKey);

        var subject = $"Test Execution Report - {summary.Timestamp:yyyy-MM-dd}";
        var body = GenerateEmailBody(summary);

        var from = new EmailAddress("tests@company.com", "Test Automation");
        var to = new EmailAddress("team@company.com", "QA Team");

        var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);

        // Attach HTML report
        var reportBytes = File.ReadAllBytes("path/to/report.html");
        msg.AddAttachment("TestReport.html", Convert.ToBase64String(reportBytes));

        await client.SendEmailAsync(msg);
    }

    private string GenerateEmailBody(TestResultSummary summary)
    {
        return $@"
            <h2>Test Execution Summary</h2>
            <table>
                <tr><td>Total:</td><td>{summary.Total}</td></tr>
                <tr><td>Passed:</td><td style='color:green'>{summary.Passed}</td></tr>
                <tr><td>Failed:</td><td style='color:red'>{summary.Failed}</td></tr>
                <tr><td>Success Rate:</td><td>{summary.SuccessRate}%</td></tr>
            </table>
        ";
    }
}
```

### Slack Notification

**Using Slack Webhooks**:
```csharp
public class SlackReporter
{
    private readonly string _webhookUrl;

    public async Task SendNotification(TestResultSummary summary)
    {
        var payload = new
        {
            text = "Test Execution Completed",
            blocks = new[]
            {
                new
                {
                    type = "section",
                    text = new
                    {
                        type = "mrkdwn",
                        text = $"*Test Execution Summary*\n" +
                               $"Total: {summary.Total}\n" +
                               $"✅ Passed: {summary.Passed}\n" +
                               $"❌ Failed: {summary.Failed}\n" +
                               $"Success Rate: {summary.SuccessRate}%"
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        await client.PostAsync(_webhookUrl, content);
    }
}
```

## Report Analysis

### Trend Analysis

**Tracking Over Time**:
```csharp
public class TrendAnalyzer
{
    public void AnalyzeTrends(List<TestRun> historicalRuns)
    {
        var trends = historicalRuns
            .OrderBy(r => r.Timestamp)
            .Select(r => new
            {
                Date = r.Timestamp.Date,
                SuccessRate = (double)r.Passed / r.Total * 100,
                AverageDuration = r.AverageDuration
            })
            .ToList();

        // Identify improvement or degradation
        var recentRate = trends.TakeLast(5).Average(t => t.SuccessRate);
        var previousRate = trends.SkipLast(5).TakeLast(5).Average(t => t.SuccessRate);

        if (recentRate < previousRate - 5)
        {
            Console.WriteLine("⚠️ WARNING: Test success rate declining!");
        }
    }
}
```

### Flaky Test Detection

```csharp
public class FlakyTestDetector
{
    public List<string> DetectFlakyTests(List<TestRun> runs)
    {
        var testResults = runs
            .SelectMany(r => r.Results)
            .GroupBy(t => t.TestName)
            .Select(g => new
            {
                TestName = g.Key,
                TotalRuns = g.Count(),
                Passed = g.Count(t => t.Status == "Passed"),
                Failed = g.Count(t => t.Status == "Failed")
            })
            .Where(t => t.Passed > 0 && t.Failed > 0)  // Test has both passes and failures
            .Where(t => (double)t.Failed / t.TotalRuns >= 0.1)  // Fails at least 10% of the time
            .Select(t => t.TestName)
            .ToList();

        return testResults;
    }
}
```

### Performance Analysis

```csharp
public class PerformanceAnalyzer
{
    public void AnalyzePerformance(List<TestResult> results)
    {
        var slowTests = results
            .Where(r => r.Duration > TimeSpan.FromMinutes(1))
            .OrderByDescending(r => r.Duration)
            .Take(10)
            .ToList();

        Console.WriteLine("Top 10 Slowest Tests:");
        foreach (var test in slowTests)
        {
            Console.WriteLine($"  {test.Name}: {test.Duration.TotalSeconds}s");
        }

        var averageDuration = results.Average(r => r.Duration.TotalSeconds);
        Console.WriteLine($"\nAverage Test Duration: {averageDuration:F2}s");
    }
}
```

## Best Practices

### 1. Consistent Reporting

✅ **Do**:
- Always generate reports, even on failure
- Use consistent naming conventions
- Include environment information
- Attach screenshots on failure

❌ **Don't**:
- Skip report generation on errors
- Use random report names
- Omit critical information

### 2. Report Organization

✅ **Do**:
```
TestReports/
├── 2025-01-04/
│   ├── TestReport_Chrome_103000.html
│   ├── TestReport_Firefox_103000.html
│   └── TestReport_Edge_103000.html
├── 2025-01-05/
│   └── ...
```

❌ **Don't**:
```
TestReports/
├── report1.html
├── report2.html
├── report_final.html
└── report_final_v2.html
```

### 3. Meaningful Logging

✅ **Do**:
```csharp
ExtentReportManager.LogInfo(_test, "Navigating to login page...");
ExtentReportManager.LogPass(_test, "Successfully logged in as admin user");
ExtentReportManager.LogFail(_test, "Login failed: Invalid credentials error displayed");
```

❌ **Don't**:
```csharp
ExtentReportManager.LogInfo(_test, "Step 1");
ExtentReportManager.LogPass(_test, "Passed");
ExtentReportManager.LogFail(_test, "Failed");
```

### 4. Screenshot Strategy

✅ **Do**:
- Take screenshots on failure
- Take screenshots at key verification points
- Name screenshots descriptively
- Compress large screenshots

❌ **Don't**:
- Take screenshots at every step
- Use generic names (screenshot1.png)
- Store uncompressed 4K screenshots

### 5. Report Retention

✅ **Do**:
- Archive old reports
- Keep last 30 days locally
- Store critical reports permanently
- Clean up temporary files

❌ **Don't**:
- Keep all reports forever
- Delete reports immediately
- Ignore disk space

## Troubleshooting

### Issue: Reports not generated

**Solutions**:
```csharp
// Ensure FlushReport is called
[AfterTestRun]
public static void AfterTestRun()
{
    ExtentReportManager.FlushReport();
}

// Check directory permissions
Directory.CreateDirectory("TestReports");

// Verify report path
Console.WriteLine($"Report path: {ExtentReportManager.GetReportPath()}");
```

### Issue: Screenshots not attached

**Solutions**:
```csharp
// Verify screenshot path
if (File.Exists(screenshotPath))
{
    ExtentReportManager.AttachScreenshot(_test, screenshotPath);
}
else
{
    Console.WriteLine($"Screenshot not found: {screenshotPath}");
}

// Use absolute paths
var absolutePath = Path.GetFullPath(screenshotPath);
```

### Issue: HTML report not opening

**Solutions**:
```bash
# Check file exists
ls -la TestReports/

# Try different browser
firefox TestReport.html

# Check file permissions
chmod 644 TestReport.html
```

---

**For more information**, refer to:
- [Main README](README.md)
- [Azure DevOps Pipeline Guide](AZURE_DEVOPS_PIPELINE_GUIDE.md)
- [UITests README](UITests/README.md)
