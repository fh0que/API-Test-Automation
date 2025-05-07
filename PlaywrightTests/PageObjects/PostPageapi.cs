using PlaywrightTests.Utilities;
using System.Text.Json;

namespace PlaywrightTests.PageObjects;

public class PostPage
{
    private readonly ApiClient _apiClient;

    public PostPage(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PostData> CreatePostAsync(PostData post)
    {
        var response = await _apiClient.PostAsync("/posts", post);
        return await response.JsonAsync<PostData>();
    }

    public async Task<CommentData> CreateCommentAsync(CommentData comment)
    {
        var response = await _apiClient.PostAsync("/comments", comment);
        return await response.JsonAsync<CommentData>();
    }
} 


   