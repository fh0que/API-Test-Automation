# Setting Up a New API Testing Project

This guide will walk you through creating a new API testing project from scratch using Playwright, xUnit, and ExtentReports.

## Step 1: Create a New .NET Project

1. **Create a new xUnit project**
   ```bash
   dotnet new xunit -n PlaywrightTests
   cd PlaywrightTests
   ```

2. **Add required NuGet packages**
   ```bash
   # Add Playwright
   dotnet add package Microsoft.Playwright
   dotnet add package Microsoft.Playwright.NUnit

   # Add ExtentReports
   dotnet add package ExtentReports
   dotnet add package ExtentReports.SparkReporter

   # Add other utilities
   dotnet add package DotNetEnv
   dotnet add package Newtonsoft.Json
   ```

## Step 2: Install Playwright

1. **Install Playwright browsers**
   ```bash
   # For Windows
   pwsh bin/Debug/net6.0/playwright.ps1 install

   # For Linux/macOS
   ./bin/Debug/net6.0/playwright.sh install
   ```

2. **Verify installation**
   ```bash
   dotnet build
   ```

## Step 3: Project Structure Setup

1. **Create the following directory structure**
   ```
   PlaywrightTests/
   ├── test/                    # Test classes
   ├── Utilities/              # Helper classes
   ├── Reports/               # Test reports
   ├── Screenshots/          # Screenshots
   └── .env                  # Environment config
   ```

2. **Create necessary directories**
   ```bash
   mkdir test
   mkdir Utilities
   mkdir Reports
   mkdir Screenshots
   ```

## Step 4: Create Base Classes

1. **Create ApiClient.cs in Utilities folder**
   ```csharp
   using Microsoft.Playwright;
   using System.Threading.Tasks;

   namespace PlaywrightTests.Utilities
   {
       public class ApiClient
       {
           private readonly IAPIRequestContext _context;
           private readonly string _baseUrl;
           private readonly string _authToken;

           public ApiClient(IAPIRequestContext context, string baseUrl, string authToken)
           {
               _context = context;
               _baseUrl = baseUrl;
               _authToken = authToken;
           }

           public async Task<IAPIResponse> GetAsync(string endpoint)
           {
               return await _context.GetAsync($"{_baseUrl}/{endpoint}");
           }

           public async Task<IAPIResponse> PostAsync(string endpoint, object data)
           {
               return await _context.PostAsync($"{_baseUrl}/{endpoint}", new APIRequestContextOptions
               {
                   Data = data
               });
           }

           // Add other HTTP methods as needed
       }
   }
   ```

2. **Create ExtentManager.cs in Utilities folder**
   ```csharp
   using AventStack.ExtentReports;
   using AventStack.ExtentReports.Reporter;
   using System;
   using System.IO;

   namespace PlaywrightTests.Utilities
   {
       public static class ExtentManager
       {
           private static ExtentReports? _extent;
           private static ExtentTest? _test;
           private static readonly string ReportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Reports");
           private static readonly string Timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
           private static readonly string ReportPath = Path.Combine(ReportDirectory, $"extent-report_{Timestamp}.html");

           public static ExtentReports GetExtent()
           {
               if (_extent != null) return _extent;

               _extent = new ExtentReports();
               var sparkReporter = new ExtentSparkReporter(ReportPath);
               _extent.AttachReporter(sparkReporter);
               return _extent;
           }

           public static ExtentTest CreateTest(string testName)
           {
               _test = GetExtent().CreateTest(testName);
               return _test;
           }

           // Add logging methods
           public static void LogInfo(string message) => _test?.Info(message);
           public static void LogPass(string message) => _test?.Pass(message);
           public static void LogFail(string message) => _test?.Fail(message);
           public static void Flush() => _extent?.Flush();
       }
   }
   ```

3. **Create TBase.cs in Utilities folder**
   ```csharp
   using Microsoft.Playwright;
   using DotNetEnv;
   using Xunit;
   using AventStack.ExtentReports;
   using System;
   using System.Threading.Tasks;

   namespace PlaywrightTests.Utilities
   {
       public class TBase : IAsyncLifetime
       {
           protected IAPIRequestContext ApiRequestContext;
           protected IPlaywright PlaywrightInstance;
           protected readonly string BaseUrl;
           protected readonly string AuthToken;
           protected ApiClient ApiClient;
           protected ExtentTest Test;

           public TBase()
           {
               const string envPath = ".env";
               if (!File.Exists(envPath))
               {
                   throw new FileNotFoundException("Environment file not found", envPath);
               }

               Env.Load(envPath);
               BaseUrl = Env.GetString("BASE_URL") ?? throw new InvalidOperationException("BASE_URL not found");
               AuthToken = Env.GetString("AUTH_TOKEN") ?? throw new InvalidOperationException("AUTH_TOKEN not found");
               
               InitializePlaywright();
               InitializeApiClient();
           }

           private void InitializePlaywright()
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

           private void InitializeApiClient()
           {
               ApiClient = new ApiClient(ApiRequestContext, BaseUrl, AuthToken);
           }

           public async Task InitializeAsync()
           {
               ExtentManager.GetExtent();
           }

           public async Task DisposeAsync()
           {
               if (ApiRequestContext != null)
               {
                   await ApiRequestContext.DisposeAsync();
               }
               if (PlaywrightInstance != null)
               {
                   PlaywrightInstance.Dispose();
               }
               ExtentManager.Flush();
           }

           // Add logging methods
           protected void LogInfo(string message) => ExtentManager.LogInfo(message);
           protected void LogPass(string message) => ExtentManager.LogPass(message);
           protected void LogFail(string message) => ExtentManager.LogFail(message);
       }
   }
   ```

## Step 5: Create Environment File

1. **Create .env file in project root**
   ```
   BASE_URL=your_api_base_url
   AUTH_TOKEN=your_auth_token
   ```

## Step 6: Create Sample Test

1. **Create PostRequestTests.cs in test folder**
   ```csharp
   using System.Threading.Tasks;
   using Xunit;
   using System;
   using System.Text.Json;
   using PlaywrightTests.Utilities;

   namespace PlaywrightTests.test
   {
       public class PostRequestTests : TBase
       {
           [Fact]
           public async Task CreatePost_ShouldReturn201()
           {
               try
               {
                   Test = ExtentManager.CreateTest(nameof(CreatePost_ShouldReturn201));
                   LogInfo($"Starting test: {nameof(CreatePost_ShouldReturn201)}");

                   var post = new { title = "foo", body = "bar", userId = 1 };
                   var response = await ApiClient.PostAsync("posts", post);
                   
                   Assert.Equal(201, response.Status);
                   LogPass($"Test {nameof(CreatePost_ShouldReturn201)} completed successfully");
               }
               catch (Exception ex)
               {
                   LogFail($"Test {nameof(CreatePost_ShouldReturn201)} failed: {ex.Message}");
                   throw;
               }
           }
       }
   }
   ```

## Step 7: Run Tests

1. **Build and run tests**
   ```bash
   dotnet build
   dotnet test
   ```

## Step 8: View Reports

1. **Check the Reports directory**
   - Open `Reports/extent-report_[timestamp].html` in a web browser
   - Review test results, logs, and any failures

## Additional Features to Implement

1. **Add Screenshot Support**
   - Implement screenshot capture in TBase
   - Add screenshot attachment to reports

2. **Add Request/Response Logging**
   - Enhance ApiClient with detailed logging
   - Add request/response details to reports

3. **Add Schema Validation**
   - Implement JSON schema validation
   - Add validation results to reports

4. **Add Parallel Test Execution**
   - Configure xUnit for parallel execution
   - Handle thread-safe reporting

## Troubleshooting

1. **Playwright Installation Issues**
   ```bash
   # Reinstall Playwright
   pwsh bin/Debug/net6.0/playwright.ps1 install --force
   ```

2. **Build Issues**
   ```bash
   # Clean and rebuild
   dotnet clean
   dotnet build
   ```

3. **Test Execution Issues**
   - Verify .env file exists and contains correct values
   - Check API endpoint accessibility
   - Verify authentication token validity

## Next Steps

1. Add more test cases
2. Implement additional HTTP methods
3. Add data-driven testing
4. Implement CI/CD integration
5. Add custom reporting features 