using System.Threading.Tasks;
using Xunit;
using System;
using System.Text.Json;
using PlaywrightTests.Utilities;

public class PutRequestTests : TBase
{
    private readonly PostPage _postPage;

    public PutRequestTests()
    {
        _postPage = new PostPage(ApiClient);
    }

    [Fact]
    public async Task UpdatePost_ShouldReturn200()
    {
        try
        {
            Test = ExtentManager.CreateTest(nameof(UpdatePost_ShouldReturn200));
            LogInfo($"Starting test: {nameof(UpdatePost_ShouldReturn200)}");

            var post = new { title = "updated title", body = "updated body", userId = 1 };
            var response = await _postPage.UpdatePostAsync(1, post);
            
            LogRequest("PUT", "posts/1", JsonSerializer.Serialize(post));
            LogResponse(response.Status, await response.TextAsync());
            
            Assert.Equal(200, response.Status);
            LogPass($"Test {nameof(UpdatePost_ShouldReturn200)} completed successfully");
        }
        catch (Exception ex)
        {
            LogFail($"Test {nameof(UpdatePost_ShouldReturn200)} failed: {ex.Message}");
            throw;
        }
    }
}
