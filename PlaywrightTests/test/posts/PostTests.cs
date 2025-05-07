using Xunit;
using PlaywrightTests.Utilities;
using PlaywrightTests.PageObjects;

namespace PlaywrightTests.test.posts;

public class PostTests : TBase
{
    private readonly PostsApi _postsApi;

    public PostTests()
    {
        _postsApi = new PostsApi(ApiClient);
    }

    [Fact]
    public async Task CreateValidPost_ShouldSucceed()
    {
        try
        {
            Test = ExtentManager.CreateTest(nameof(CreateValidPost_ShouldSucceed));
            LogInfo($"Starting test: {nameof(CreateValidPost_ShouldSucceed)}");

            // Arrange
            var validPost = TestData.TestDataContent.Posts.ValidPost;
            LogInfo($"Creating post with title: {validPost.Title}");

            // Act
            var responseData = await _postsApi.CreatePostAsync(validPost);
            
            // Assert
            Assert.NotNull(responseData);
            Assert.Equal(validPost.Title, responseData.Title);
            Assert.Equal(validPost.Body, responseData.Body);
            Assert.Equal(validPost.UserId, responseData.UserId);
            
            LogPass($"Test {nameof(CreateValidPost_ShouldSucceed)} completed successfully");
        }
        catch (Exception ex)
        {
            LogFail($"Test {nameof(CreateValidPost_ShouldSucceed)} failed: {ex.Message}");
            throw;
        }
    }

    [Fact]
    public async Task CreateInvalidPost_ShouldFail()
    {
        try
        {
            Test = ExtentManager.CreateTest(nameof(CreateInvalidPost_ShouldFail));
            LogInfo($"Starting test: {nameof(CreateInvalidPost_ShouldFail)}");

            // Arrange
            var invalidPost = TestData.TestDataContent.Posts.InvalidPost;
            LogInfo($"Attempting to create invalid post with empty title");

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => 
                await _postsApi.CreatePostAsync(invalidPost));
            
            LogPass($"Test {nameof(CreateInvalidPost_ShouldFail)} completed successfully");
        }
        catch (Exception ex)
        {
            LogFail($"Test {nameof(CreateInvalidPost_ShouldFail)} failed: {ex.Message}");
            throw;
        }
    }

    [Fact]
    public async Task CreateValidComment_ShouldSucceed()
    {
        try
        {
            Test = ExtentManager.CreateTest(nameof(CreateValidComment_ShouldSucceed));
            LogInfo($"Starting test: {nameof(CreateValidComment_ShouldSucceed)}");

            // Arrange
            var validComment = TestData.TestDataContent.Comments.ValidComment;
            LogInfo($"Creating comment for post {validComment.PostId}");

            // Act
            var response = await _postsApi.CreateCommentAsync(validComment);
            
            // Assert
            Assert.NotNull(response);
            Assert.Equal(validComment.PostId, response.PostId);
            Assert.Equal(validComment.Name, response.Name);
            Assert.Equal(validComment.Email, response.Email);
            Assert.Equal(validComment.Body, response.Body);
            
            LogPass($"Test {nameof(CreateValidComment_ShouldSucceed)} completed successfully");
        }
        catch (Exception ex)
        {
            LogFail($"Test {nameof(CreateValidComment_ShouldSucceed)} failed: {ex.Message}");
            throw;
        }
    }

    [Fact]
    public async Task CreateInvalidComment_ShouldFail()
    {
        try
        {
            Test = ExtentManager.CreateTest(nameof(CreateInvalidComment_ShouldFail));
            LogInfo($"Starting test: {nameof(CreateInvalidComment_ShouldFail)}");

            // Arrange
            var invalidComment = TestData.TestDataContent.Comments.InvalidComment;
            LogInfo($"Attempting to create invalid comment with invalid email");

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => 
                await _postsApi.CreateCommentAsync(invalidComment));
            
            LogPass($"Test {nameof(CreateInvalidComment_ShouldFail)} completed successfully");
        }
        catch (Exception ex)
        {
            LogFail($"Test {nameof(CreateInvalidComment_ShouldFail)} failed: {ex.Message}");
            throw;
        }
    }
} 