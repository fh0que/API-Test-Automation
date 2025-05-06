using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace PlaywrightTests.Utilities
{
    public static class ExtentManager
    {
        private static ExtentReports? _extent;
        private static ExtentTest? _test;
        private static readonly string ReportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Reports");
        private static readonly string Timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        private static readonly string ReportPath = Path.Combine(ReportDirectory, $"extent-report_{Timestamp}.html");
        private static readonly string JsonReportPath = Path.Combine(ReportDirectory, $"extent-report_{Timestamp}.json");
        private static readonly object _lock = new object();
        private static readonly List<TestResult> _testResults = new();

        private class TestResult
        {
            public string Name { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string StartTime { get; set; } = string.Empty;
            public string EndTime { get; set; } = string.Empty;
            public List<string> Logs { get; set; } = new();
        }

        public static ExtentReports GetExtent()
        {
            if (_extent != null) return _extent;

            lock (_lock)
            {
                if (_extent != null) return _extent;

                _extent = new ExtentReports();
                var sparkReporter = new ExtentSparkReporter(ReportPath);
                
                // Configure the reporter
                sparkReporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Dark;
                sparkReporter.Config.DocumentTitle = "API Test Report";
                sparkReporter.Config.ReportName = "API Test Execution Report";
                sparkReporter.Config.TimelineEnabled = true;
                
                _extent.AttachReporter(sparkReporter);
                
                // Add system information
                _extent.AddSystemInfo("Environment", "QA");
                _extent.AddSystemInfo("Browser", "Playwright");
                _extent.AddSystemInfo("Framework", "xUnit");
                
                return _extent;
            }
        }

        public static ExtentTest CreateTest(string testName)
        {
            lock (_lock)
            {
                _test = GetExtent().CreateTest(testName);
                _testResults.Add(new TestResult 
                { 
                    Name = testName,
                    StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
                return _test;
            }
        }

        public static void LogInfo(string message)
        {
            lock (_lock)
            {
                _test?.Info(message);
                AddLog("INFO", message);
            }
        }

        public static void LogPass(string message)
        {
            lock (_lock)
            {
                _test?.Pass(message);
                AddLog("PASS", message);
                UpdateTestStatus("PASS");
            }
        }

        public static void LogFail(string message)
        {
            lock (_lock)
            {
                _test?.Fail(message);
                AddLog("FAIL", message);
                UpdateTestStatus("FAIL");
            }
        }

        public static void LogWarning(string message)
        {
            lock (_lock)
            {
                _test?.Warning(message);
                AddLog("WARNING", message);
            }
        }

        public static void LogError(string message)
        {
            lock (_lock)
            {
                _test?.Fail(message);
                AddLog("ERROR", message);
                UpdateTestStatus("FAIL");
            }
        }

        public static void LogRequest(string method, string url, string? body)
        {
            lock (_lock)
            {
                _test?.Info($"Request: {method} {url}");
                AddLog("REQUEST", $"Request: {method} {url}");
                if (!string.IsNullOrEmpty(body))
                {
                    _test?.Info($"Request Body: {body}");
                    AddLog("REQUEST", $"Request Body: {body}");
                }
            }
        }

        public static void LogResponse(int statusCode, string body)
        {
            lock (_lock)
            {
                _test?.Info($"Response Status: {statusCode}");
                _test?.Info($"Response Body: {body}");
                AddLog("RESPONSE", $"Status: {statusCode}");
                AddLog("RESPONSE", $"Body: {body}");
            }
        }

        private static void AddLog(string level, string message)
        {
            var currentTest = _testResults.Find(t => t.Name == _test?.Model.Name);
            if (currentTest != null)
            {
                currentTest.Logs.Add($"[{level}] {message}");
            }
        }

        private static void UpdateTestStatus(string status)
        {
            var currentTest = _testResults.Find(t => t.Name == _test?.Model.Name);
            if (currentTest != null)
            {
                currentTest.Status = status;
                currentTest.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public static void Flush()
        {
            lock (_lock)
            {
                _extent?.Flush();
                // Write JSON report
                var json = JsonSerializer.Serialize(_testResults, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(JsonReportPath, json);
            }
        }

        public static void AttachFailureReport(Exception ex, string? screenshotPath = null)
        {
            lock (_lock)
            {
                if (_test == null) return;

                // Log the exception details
                _test.Fail($"Exception Type: {ex.GetType().Name}");
                _test.Fail($"Error Message: {ex.Message}");
                _test.Fail($"Stack Trace: {ex.StackTrace}");

                // Add to JSON report
                AddLog("FAIL", $"Exception Type: {ex.GetType().Name}");
                AddLog("FAIL", $"Error Message: {ex.Message}");
                AddLog("FAIL", $"Stack Trace: {ex.StackTrace}");

                // Handle inner exception if present
                if (ex.InnerException != null)
                {
                    _test.Fail($"Inner Exception: {ex.InnerException.Message}");
                    AddLog("FAIL", $"Inner Exception: {ex.InnerException.Message}");
                }

                // Attach screenshot if provided
                if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
                {
                    try
                    {
                        _test.AddScreenCaptureFromPath(screenshotPath, "Failure Screenshot");
                        AddLog("SCREENSHOT", $"Screenshot attached: {screenshotPath}");
                    }
                    catch (Exception screenshotEx)
                    {
                        _test.Warning($"Failed to attach screenshot: {screenshotEx.Message}");
                        AddLog("WARNING", $"Failed to attach screenshot: {screenshotEx.Message}");
                    }
                }

                UpdateTestStatus("FAIL");
            }
        }

        public static void AttachRequestResponse(string requestDetails, string responseDetails)
        {
            lock (_lock)
            {
                if (_test == null) return;

                _test.Info("Request Details:");
                _test.Info(requestDetails);
                _test.Info("Response Details:");
                _test.Info(responseDetails);

                AddLog("REQUEST_DETAILS", requestDetails);
                AddLog("RESPONSE_DETAILS", responseDetails);
            }
        }
    }
} 