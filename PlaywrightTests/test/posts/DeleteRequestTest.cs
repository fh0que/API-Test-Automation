using System.Threading.Tasks;
using Xunit;
using System;
using PlaywrightTests.Utilities;

public class DeleteRequestTests : TBase
{
    private readonly PostPage _postPage;

    public DeleteRequestTests()
    {
        _postPage = new PostPage(ApiClient);
    }

    [Fact]
    public async Task DeletePost_ShouldReturn200()
    {
        var response = await _postPage.DeletePostAsync(1);
        Assert.Equal(200, response.Status);
    }
}
