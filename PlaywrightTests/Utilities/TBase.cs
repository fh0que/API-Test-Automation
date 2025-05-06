using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using DotNetEnv;
using Xunit;
using AventStack.ExtentReports;
using PlaywrightTests.Utilities;
using System.IO;

public class TBase : IAsyncLifetime
{
    protected IAPIRequestContext ApiRequestContext;
    protected IPlaywright PlaywrightInstance;
    protected readonly string BaseUrl;
    protected readonly string AuthToken;
    protected ApiClient ApiClient;
    protected ExtentTest Test;
    protected IBrowser Browser;
    protected IPage Page;
    private readonly string ScreenshotDirectory;

    public TBase()
    {
        const string envPath = @"C:\Users\fazlul\Desktop\API\APIC\PlaywrightTests\.env";
        if (!File.Exists(envPath))
        {
            throw new FileNotFoundException("Environment file not found", envPath);
        }

        Env.Load(envPath);
        BaseUrl = Env.GetString("BASE_URL") ?? throw new InvalidOperationException("BASE_URL not found in environment");
        AuthToken = Env.GetString("AUTH_TOKEN") ?? throw new InvalidOperationException("AUTH_TOKEN not found in environment");
        
        ScreenshotDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Screenshots");
        if (!Directory.Exists(ScreenshotDirectory))
        {
            Directory.CreateDirectory(ScreenshotDirectory);
        }

        InitializePlaywright();
        InitializeApiClient();
    }

    private void InitializePlaywright()
    {
        try
        {
            PlaywrightInstance = Playwright.CreateAsync().GetAwaiter().GetResult();
            ApiRequestContext = PlaywrightInstance.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = BaseUrl,
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {AuthToken}" },
                    { "Content-Type", "application/json" }
                }
            }).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize Playwright", ex);
        }
    }

    private void InitializeApiClient()
    {
        ApiClient = new ApiClient(ApiRequestContext, BaseUrl, AuthToken);
    }

    private async Task InitializeBrowser()
    {
        try
        {
            Browser = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });
            var context = await Browser.NewContextAsync();
            Page = await context.NewPageAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize browser", ex);
        }
    }

    public async Task InitializeAsync()
    {
        // Initialize ExtentReports if needed
        ExtentManager.GetExtent();
        await InitializeBrowser();
    }

    public async Task DisposeAsync()
    {
        if (Page != null)
        {
            await Page.CloseAsync();
        }
        if (Browser != null)
        {
            await Browser.CloseAsync();
        }
        if (ApiRequestContext != null)
        {
            await ApiRequestContext.DisposeAsync();
        }
        if (PlaywrightInstance != null)
        {
            PlaywrightInstance.Dispose();
        }
        // Flush the ExtentReports
        ExtentManager.Flush();
    }

    protected void LogRequest(string method, string url, string? body)
    {
        ExtentManager.LogRequest(method, url, body);
    }

    protected void LogResponse(int statusCode, string body)
    {
        ExtentManager.LogResponse(statusCode, body);
    }

    protected void LogInfo(string message)
    {
        ExtentManager.LogInfo(message);
    }

    protected void LogPass(string message)
    {
        ExtentManager.LogPass(message);
    }

    protected void LogFail(string message)
    {
        ExtentManager.LogFail(message);
    }

    protected void LogWarning(string message)
    {
        ExtentManager.LogWarning(message);
    }

    protected void AttachFailureReport(Exception ex, string? screenshotPath = null)
    {
        ExtentManager.AttachFailureReport(ex, screenshotPath);
    }

    protected void AttachRequestResponse(string requestDetails, string responseDetails)
    {
        ExtentManager.AttachRequestResponse(requestDetails, responseDetails);
    }

    protected async Task<string> CaptureScreenshot(string testName)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string screenshotPath = Path.Combine(ScreenshotDirectory, $"{testName}_{timestamp}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });
            return screenshotPath;
        }
        catch (Exception ex)
        {
            LogWarning($"Failed to capture screenshot: {ex.Message}");
            return string.Empty;
        }
    }

    protected async Task AttachFailureReportWithScreenshot(Exception ex, string testName)
    {
        string screenshotPath = await CaptureScreenshot(testName);
        AttachFailureReport(ex, screenshotPath);
    }
}
