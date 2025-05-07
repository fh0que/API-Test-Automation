using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using DotNetEnv;
using Xunit;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.PageObjects
{
    public class CommentsPage
    {
        
        private readonly ApiClient _apiClient;

        public CommentsPage (ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

    

        public async Task<IAPIResponse> GetCommentAsync(int commentId)
        {
            return await _apiClient.GetAsync($"/comments/{commentId}");
        }

        public async Task<IAPIResponse> CreateCommentAsync(object comment)
        {
            return await _apiClient.PostAsync("/comments", comment);
        }

        public async Task<IAPIResponse> UpdateCommentAsync(int commentId, object comment)
        {
            return await _apiClient.PutAsync($"/comments/{commentId}", comment);
        }

        public async Task<IAPIResponse> DeleteCommentAsync(int commentId)
        {
            return await _apiClient.DeleteAsync($"/comments/{commentId}");
        }
    }
}
