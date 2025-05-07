using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Serilog;

namespace PlaywrightTests.Utilities;

public class ApiClient
{
    private readonly IAPIRequestContext _apiRequestContext;
    private readonly string _baseUrl;
    private readonly string _authToken;

    public ApiClient(IAPIRequestContext apiRequestContext, string baseUrl, string authToken)
    {
        _apiRequestContext = apiRequestContext;
        _baseUrl = baseUrl;
        _authToken = authToken;

        // Configure logging
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }

   /* private void LogRequest(string method, string url, object? body = null)
    {
        Log.Information($"Request: {method} {url} | Body: {JsonSerializer.Serialize(body)}");
    }*/
    private void LogRequest(string method, string url, object? body = null)
    {
        Log.Information($"Request: {method} {url} | Body: {JsonSerializer.Serialize(body)}");
    }

    private void LogResponse(IAPIResponse response)
    {
        // Use TextAsync to log the response body as string
        var responseBody = response.TextAsync().Result;
        Log.Information($"Response Status: {response.Status} | Body: {responseBody}");
    }

    private void LogError(Exception ex)
    {
        Log.Error($"Error: {ex.Message}");
    }

    public async Task<IAPIResponse> GetAsync(string endpoint)
    {
        try
        {
            var url = $"{_baseUrl}{endpoint}";
            LogRequest("GET", url);
            var response = await _apiRequestContext.GetAsync(endpoint);
            LogResponse(response);
            return response;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new ApplicationException($"GET request failed for {endpoint}.", ex);
        }
    }

    public async Task<IAPIResponse> PostAsync(string endpoint, object body)
    {
        try
        {
            var url = $"{_baseUrl}{endpoint}";
            LogRequest("POST", url, body);
            var response = await _apiRequestContext.PostAsync(endpoint, new APIRequestContextOptions
            {
                Data = JsonSerializer.Serialize(body),
                Headers = new Dictionary<string, string> { { "Authorization", $"Bearer {_authToken}" }, { "Content-Type", "application/json" } }
            });
            LogResponse(response);
            return response;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new ApplicationException($"POST request failed for {endpoint}.", ex);
        }
    }

    public async Task<IAPIResponse> PutAsync(string endpoint, object body)
    {
        try
        {
            var url = $"{_baseUrl}{endpoint}";
            LogRequest("PUT", url, body);
            var response = await _apiRequestContext.PutAsync(endpoint, new APIRequestContextOptions
            {
                Data = JsonSerializer.Serialize(body),
                Headers = new Dictionary<string, string> { { "Authorization", $"Bearer {_authToken}" }, { "Content-Type", "application/json" } }
            });
            LogResponse(response);
            return response;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new ApplicationException($"PUT request failed for {endpoint}.", ex);
        }
    }

    public async Task<IAPIResponse> DeleteAsync(string endpoint)
    {
        try
        {
            var url = $"{_baseUrl}{endpoint}";
            LogRequest("DELETE", url);
            var response = await _apiRequestContext.DeleteAsync(endpoint);
            LogResponse(response);
            return response;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new ApplicationException($"DELETE request failed for {endpoint}.", ex);
        }
    }

    public async Task<PostData> CreatePostAsync(PostData post)
    {
        var response = await _apiRequestContext.PostAsync($"{_baseUrl}/posts", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(post)
        });
        return await response.JsonAsync<PostData>();
    }

    public async Task<CommentData> CreateCommentAsync(CommentData comment)
    {
        var response = await _apiRequestContext.PostAsync($"{_baseUrl}/comments", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(comment)
        });
        return await response.JsonAsync<CommentData>();
    }
}
