using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.Playwright;
using DotNetEnv;
using Xunit;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.PageObjects
{
    public class PostsApi
    {
        private readonly ApiClient _apiClient;

        public PostsApi(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IAPIResponse> GetPostAsync(int postId)
        {
            return await _apiClient.GetAsync($"/posts/{postId}");
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

        public async Task<IAPIResponse> UpdatePostAsync(int postId, object post)
        {
            return await _apiClient.PutAsync($"/posts/{postId}", post);
        }

        public async Task<IAPIResponse> DeletePostAsync(int postId)
        {
            return await _apiClient.DeleteAsync($"/posts/{postId}");
        }
    }
} 

