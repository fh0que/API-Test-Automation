using System.Threading.Tasks;
using Xunit;
using System;
using System.Text.Json;
using PlaywrightTests.Utilities;
using PlaywrightTests.PageObjects;

namespace PlaywrightTests.test.posts;

public class PutRequestTest : TBase
{
    private readonly PostsApi _postsApi;    

    public PutRequestTest()
    {
        _postsApi = new PostsApi(ApiClient);
    }

    [Fact]
    public async Task UpdatePost_ShouldReturn200()
    {
        try
        {
            Test = ExtentManager.CreateTest(nameof(UpdatePost_ShouldReturn200));
            LogInfo($"Starting test: {nameof(UpdatePost_ShouldReturn200)}");

            var post = new { title = "updated title", body = "updated body", userId = 1 };
            var response = await _postsApi.UpdatePostAsync(1, post);
            
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
