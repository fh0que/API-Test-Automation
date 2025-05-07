using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
using Serilog;
using PlaywrightTests.PageObjects;

namespace PlaywrightTests.test.posts;

public class GetRequestTests : TBase
{
    private readonly PostsApi _postsApi;

    public GetRequestTests()
    {
        _postsApi = new PostsApi(ApiClient);
    }

    [Fact]
    public async Task GetPost_ShouldReturn200()
    {
        var response = await _postsApi.GetPostAsync(1);
        Assert.Equal(200, response.Status);
        if (response.Status != 200)
        {
            Log.Information($"Failed to get post with ID 1. Response: {response.Status}");
        }
    }

    [Fact]
    public async Task GetPost_ShouldMatchSchema()
    {
        // Get response from API
        var response = await _postsApi.GetPostAsync(1);
        Assert.Equal(200, response.Status);

        // Parse response body
        string responseBody = await response.TextAsync();

        // Load schema from file
        string schemaJson = File.ReadAllText("schemas/post_schema.json");

        // Validate response against schema
        bool isValid = JsonSchemaValidator.Validate(responseBody, schemaJson, out string validationErrors);

        // Assert schema validation passes
        Assert.True(isValid, $"Schema validation failed: {validationErrors}");

        // Log validation errors if any
        if (!isValid)
        {
            Log.Error($"Schema validation failed for post ID 1. Errors: {validationErrors}");
        }
    }
}
