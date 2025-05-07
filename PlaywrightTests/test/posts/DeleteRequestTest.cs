using System.Threading.Tasks;
using Xunit;
using System;
using PlaywrightTests.Utilities;
using PlaywrightTests.PageObjects;  

namespace PlaywrightTests.test.posts;

public class DeleteRequestTests : TBase
{
    private readonly PostsApi _postsApi;

    public DeleteRequestTests()
    {
        _postsApi = new PostsApi(ApiClient);
    }

    [Fact]
    public async Task DeletePost_ShouldReturn200()
    {
        var response = await _postsApi.DeletePostAsync(1);
        Assert.Equal(200, response.Status);
    }
}
