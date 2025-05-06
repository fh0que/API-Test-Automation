using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PlaywrightTests.Utilities
{
    public static class TestReporter
    {
        private static readonly string ReportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Reports");
        private static readonly string Timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        private static readonly string ReportPath = Path.Combine(ReportDirectory, $"test-report_{Timestamp}.html");
        private static readonly string JsonReportPath = Path.Combine(ReportDirectory, $"test-report_{Timestamp}.json");
        private static ILogger? _logger;
        private static int _totalTests = 0;
        private static int _passedTests = 0;
        private static int _failedTests = 0;
        private static readonly List<(string Name, bool Passed, string Duration)> _testResults = new();
        private static DateTime _startTime = DateTime.Now;
        private static readonly StringBuilder _testResultsHtml = new();
        private static bool _isFirstTest = true;

        static TestReporter()
        {
            try
            {
                if (!Directory.Exists(ReportDirectory))
                {
                    Directory.CreateDirectory(ReportDirectory);
                }

                // Create HTML header with styles
                var htmlHeader = @"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Test Execution Report</title>
    <style>
        body { 
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }
        .container {
            max-width: 1200px;
            margin: 0 auto;
            background-color: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .header {
            background-color: #2c3e50;
            color: white;
            padding: 20px;
            border-radius: 6px;
            margin-bottom: 20px;
        }
        .summary {
            display: flex;
            justify-content: space-between;
            margin: 20px 0;
            padding: 15px;
            background-color: #f8f9fa;
            border-radius: 6px;
        }
        .summary-item {
            text-align: center;
            padding: 10px 20px;
            border-radius: 4px;
            color: white;
            font-weight: bold;
        }
        .total { background-color: #3498db; }
        .passed { background-color: #2ecc71; }
        .failed { background-color: #e74c3c; }
        .duration { background-color: #9b59b6; }
        .log-entry {
            padding: 10px;
            margin: 5px 0;
            border-radius: 4px;
            border-left: 4px solid #3498db;
        }
        .timestamp {
            color: #7f8c8d;
            font-size: 0.9em;
            margin-right: 10px;
        }
        .level {
            font-weight: bold;
            padding: 2px 6px;
            border-radius: 3px;
            margin-right: 10px;
        }
        .level-INF { background-color: #2ecc71; color: white; }
        .level-ERR { background-color: #e74c3c; color: white; }
        .level-DBG { background-color: #3498db; color: white; }
        .message {
            color: #2c3e50;
        }
        .test-start {
            background-color: #f8f9fa;
            border-left-color: #3498db;
        }
        .test-end {
            background-color: #f8f9fa;
            border-left-color: #2ecc71;
        }
        .test-fail {
            background-color: #fdf7f7;
            border-left-color: #e74c3c;
        }
        .request {
            background-color: #f8f9fa;
            border-left-color: #9b59b6;
        }
        .response {
            background-color: #f8f9fa;
            border-left-color: #f1c40f;
        }
        .test-results {
            margin: 20px 0;
            padding: 15px;
            background-color: #f8f9fa;
            border-radius: 6px;
        }
        .test-result-item {
            padding: 10px;
            margin: 5px 0;
            border-radius: 4px;
            background-color: white;
            border-left: 4px solid #3498db;
        }
        .test-result-passed {
            border-left-color: #2ecc71;
        }
        .test-result-failed {
            border-left-color: #e74c3c;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Test Execution Report</h1>
            <p>Generated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"</p>
        </div>
        <div class='content'>";

                // Write header to file
                File.WriteAllText(ReportPath, htmlHeader);

                _logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File(JsonReportPath, outputTemplate: "{Message}{NewLine}")
                    .WriteTo.File(ReportPath, 
                        outputTemplate: "<div class='log-entry {Level:u3}'><span class='timestamp'>{Timestamp:yyyy-MM-dd HH:mm:ss}</span> <span class='level level-{Level:u3}'>{Level:u3}</span> <span class='message'>{Message}</span></div>{NewLine}",
                        shared: true,
                        rollingInterval: RollingInterval.Infinite)
                    .CreateLogger();

                LogTestStart("Test Run Started");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing TestReporter: {ex.Message}");
            }
        }

        public static void LogTestStart(string testName)
        {
            try
            {
                _totalTests++;
                _logger?.Information($"<div class='test-start'>=== Test Started: {testName} ===</div>");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging test start: {ex.Message}");
            }
        }

        public static void LogTestEnd(string testName, bool passed)
        {
            try
            {
                var status = passed ? "PASSED" : "FAILED";
                var className = passed ? "test-end" : "test-fail";
                if (passed) _passedTests++; else _failedTests++;
                
                var duration = DateTime.Now - _startTime;
                _testResults.Add((testName, passed, duration.ToString(@"hh\:mm\:ss")));
                
                _logger?.Information($"<div class='{className}'>=== Test Ended: {testName} - {status} ===</div>");
                
                // Add to test results HTML
                var resultClass = passed ? "test-result-passed" : "test-result-failed";
                _testResultsHtml.AppendLine($"<div class='test-result-item {resultClass}'>{testName} - {status} ({duration.TotalSeconds:F1}s)</div>");

                // Write summary after each test
                var summaryHtml = $@"
        </div>
        <div class='summary'>
            <div class='summary-item total'>Total Tests: {_totalTests}</div>
            <div class='summary-item passed'>Passed: {_passedTests}</div>
            <div class='summary-item failed'>Failed: {_failedTests}</div>
            <div class='summary-item duration'>Duration: {duration.TotalSeconds:F1}s</div>
        </div>
        <div class='test-results'>
            <h2>Test Results</h2>
            {_testResultsHtml}
        </div>
    </div>
</body>
</html>";

                // If it's the first test, write the entire file
                if (_isFirstTest)
                {
                    File.WriteAllText(ReportPath, File.ReadAllText(ReportPath) + summaryHtml);
                    _isFirstTest = false;
                }
                else
                {
                    // For subsequent tests, update the summary section
                    var content = File.ReadAllText(ReportPath);
                    var startIndex = content.IndexOf("<div class='summary'>");
                    if (startIndex >= 0)
                    {
                        var endIndex = content.IndexOf("</body>");
                        if (endIndex >= 0)
                        {
                            var newContent = content.Substring(0, startIndex) + summaryHtml;
                            File.WriteAllText(ReportPath, newContent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging test end: {ex.Message}");
            }
        }

        public static void LogRequest(string method, string url, string? body)
        {
            try
            {
                _logger?.Information($"<div class='request'>Request: {method} {url} | Body: {body}</div>");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging request: {ex.Message}");
            }
        }

        public static void LogResponse(int statusCode, string body)
        {
            try
            {
                _logger?.Information($"<div class='response'>Response Status: {statusCode} | Body: {body}</div>");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging response: {ex.Message}");
            }
        }

        public static void LogError(string message, Exception? ex = null)
        {
            try
            {
                _logger?.Error(ex, $"<div class='test-fail'>{message}</div>");
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"Error logging error: {logEx.Message}");
            }
        }
    }
} 