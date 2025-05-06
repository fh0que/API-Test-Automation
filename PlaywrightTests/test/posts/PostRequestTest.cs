using System.Threading.Tasks;
using Xunit;
using System;
using System.Text.Json;
using PlaywrightTests.Utilities;    
using PlaywrightTests.PageObjects;

public class PostRequestTests : TBase
{
    private readonly PostPage _postPage;

    public PostRequestTests()
    {
        _postPage = new PostPage(ApiClient);
    }

    [Fact]
    public async Task CreatePost_ShouldReturn201()
    {
        try
        {
            Test = ExtentManager.CreateTest(nameof(CreatePost_ShouldReturn201));
            LogInfo($"Starting test: {nameof(CreatePost_ShouldReturn201)}");

            var post = new { title = "foo", body = "bar", userId = 1 };
            var response = await _postPage.CreatePostAsync(post);
            
            LogRequest("POST", "posts", JsonSerializer.Serialize(post));
            LogResponse(response.Status, await response.TextAsync());
            
            Assert.Equal(201, response.Status);
            LogPass($"Test {nameof(CreatePost_ShouldReturn201)} completed successfully");
        }
        catch (Exception ex)
        {
            await AttachFailureReportWithScreenshot(ex, nameof(CreatePost_ShouldReturn201));
            throw;
        }
    }

    [Fact]
    public async Task CreatePost_ShouldMatchSchema()
    {
        try
        {
            Test = ExtentManager.CreateTest(nameof(CreatePost_ShouldMatchSchema));
            LogInfo($"Starting test: {nameof(CreatePost_ShouldMatchSchema)}");

            var post = new { title = "foo", body = "bar", userId = 1 };
            var response = await _postPage.CreatePostAsync(post);
            
            LogRequest("POST", "posts", JsonSerializer.Serialize(post));
            LogResponse(response.Status, await response.TextAsync());
            
            Assert.Equal(201, response.Status);

            string responseBody = await response.TextAsync();
            string schemaJson = File.ReadAllText("schemas/post_schema.json");

            bool isValid = JsonSchemaValidator.Validate(responseBody, schemaJson, out string validationErrors);

            if (isValid)
            {
                LogPass("Schema validation passed");
            }
            else
            {
                LogFail($"Schema validation failed: {validationErrors}");
            }

            Assert.True(isValid, $"Schema validation failed: {validationErrors}");
        }
        catch (Exception ex)
        {
            await AttachFailureReportWithScreenshot(ex, nameof(CreatePost_ShouldMatchSchema));
            throw;
        }
    }
}
