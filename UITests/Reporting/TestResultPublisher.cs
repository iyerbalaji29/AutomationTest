using System.Xml.Linq;
using Common.Utilities;

namespace UITests.Reporting
{
    /// <summary>
    /// Publishes test results in various formats for CI/CD pipelines
    /// </summary>
    public class TestResultPublisher
    {
        private readonly string _resultsDirectory;

        public TestResultPublisher(string resultsDirectory = "TestResults")
        {
            _resultsDirectory = resultsDirectory;
            Directory.CreateDirectory(_resultsDirectory);
        }

        /// <summary>
        /// Parses TRX (Visual Studio Test Results) file and extracts test statistics
        /// </summary>
        public TestResultSummary ParseTrxResults(string trxFilePath)
        {
            if (!File.Exists(trxFilePath))
            {
                Logger.Warning($"TRX file not found: {trxFilePath}");
                return new TestResultSummary();
            }

            try
            {
                var doc = XDocument.Load(trxFilePath);
                var ns = doc.Root?.Name.Namespace;

                var resultSummary = doc.Descendants(ns + "ResultSummary").FirstOrDefault();
                var counters = resultSummary?.Element(ns + "Counters");

                var summary = new TestResultSummary
                {
                    Total = int.Parse(counters?.Attribute("total")?.Value ?? "0"),
                    Passed = int.Parse(counters?.Attribute("passed")?.Value ?? "0"),
                    Failed = int.Parse(counters?.Attribute("failed")?.Value ?? "0"),
                    Skipped = int.Parse(counters?.Attribute("notExecuted")?.Value ?? "0"),
                    ExecutionTime = resultSummary?.Attribute("outcome")?.Value ?? "Unknown"
                };

                return summary;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to parse TRX file", ex);
                return new TestResultSummary();
            }
        }

        /// <summary>
        /// Generates a summary report in JSON format for pipeline consumption
        /// </summary>
        public void GenerateJsonSummary(TestResultSummary summary, string outputPath)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(summary, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(outputPath, json);
            Logger.Info($"JSON summary generated: {outputPath}");
        }

        /// <summary>
        /// Generates a markdown summary for Azure DevOps
        /// </summary>
        public void GenerateMarkdownSummary(TestResultSummary summary, string outputPath)
        {
            var successRate = summary.Total > 0
                ? Math.Round((double)summary.Passed / summary.Total * 100, 2)
                : 0;

            var markdown = $@"# Test Execution Summary

## Overall Results

| Metric | Count |
|--------|-------|
| Total Tests | {summary.Total} |
| Passed | ✅ {summary.Passed} |
| Failed | ❌ {summary.Failed} |
| Skipped | ⏭️ {summary.Skipped} |
| Success Rate | {successRate}% |

## Status

{(summary.Failed == 0 ? "✅ **All tests passed!**" : $"❌ **{summary.Failed} test(s) failed**")}

**Execution Time:** {summary.ExecutionTime}

---
*Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}*
";

            File.WriteAllText(outputPath, markdown);
            Logger.Info($"Markdown summary generated: {outputPath}");
        }

        /// <summary>
        /// Copies test artifacts to a specific directory for publishing
        /// </summary>
        public void CopyTestArtifacts(string sourceDirectory, string targetDirectory, string pattern = "*.*")
        {
            Directory.CreateDirectory(targetDirectory);

            var files = Directory.GetFiles(sourceDirectory, pattern, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var relativePath = Path.GetRelativePath(sourceDirectory, file);
                var targetPath = Path.Combine(targetDirectory, relativePath);
                var targetDir = Path.GetDirectoryName(targetPath);

                if (!string.IsNullOrEmpty(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                File.Copy(file, targetPath, overwrite: true);
            }

            Logger.Info($"Copied {files.Length} files to {targetDirectory}");
        }
    }

    /// <summary>
    /// Test result summary model
    /// </summary>
    public class TestResultSummary
    {
        public int Total { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Skipped { get; set; }
        public string ExecutionTime { get; set; } = string.Empty;
        public string Environment { get; set; } = "Local";
        public string Browser { get; set; } = "Chrome";
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
