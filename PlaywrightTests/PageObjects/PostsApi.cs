using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using DotNetEnv;
using Xunit;


public class PostPage
{
    private readonly ApiClient _apiClient;

    public PostPage(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IAPIResponse> GetPostAsync(int postId)
    {
        return await _apiClient.GetAsync($"/posts/{postId}");
    }

    public async Task<IAPIResponse> CreatePostAsync(object post)
    {
        return await _apiClient.PostAsync("/posts", post);
    }

    public async Task<IAPIResponse> UpdatePostAsync(int postId, object post)
    {
        return await _apiClient.PutAsync($"/posts/{postId}", post);
    }

    public async Task<IAPIResponse> DeletePostAsync(int postId)
    {
        return await _apiClient.DeleteAsync($"/posts/{postId}");
    }
}
