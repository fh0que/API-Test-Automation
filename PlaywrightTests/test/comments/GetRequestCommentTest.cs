using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
using Serilog;
using System.Text.Json;
using System.Text.RegularExpressions;
using PlaywrightTests.Utilities;
using System;
using PlaywrightTests.PageObjects;

namespace PlaywrightTests.test.comments
{
    public class GetRequestCommentTest : TBase, IAsyncLifetime
    {
        private readonly CommentsPage _commentsPage;

        public GetRequestCommentTest()
        {
            _commentsPage = new CommentsPage(ApiClient);
        }

       

        [Fact]
        public async Task Getcomments_ShouldReturn200()
        {
            try
            {
                Test = ExtentManager.CreateTest(nameof(Getcomments_ShouldReturn200));
                LogInfo($"Starting test: {nameof(Getcomments_ShouldReturn200)}");

                var response = await _commentsPage.GetCommentAsync(2);
                string responseBody = await response.TextAsync();
                
                // Log request and response
                LogRequest("GET", $"comments/2", null);
                LogResponse(response.Status, responseBody);

                Assert.Equal(200, response.Status);
                LogPass($"Successfully retrieved comment with ID 2. Status: {response.Status}");

                // Log raw response for debugging
                LogInfo($"Raw response body: {responseBody}");

                // Validate email format in response
                var comment = JsonSerializer.Deserialize<CommentResponse>(responseBody, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                if (comment == null)
                {
                    LogFail($"Failed to deserialize response: {responseBody}");
                    throw new InvalidOperationException("Failed to deserialize comment response");
                }

                LogInfo($"Deserialized comment: {JsonSerializer.Serialize(comment)}");
                ValidateEmailFormat(comment.Email);
                Assert.Equal("Jayne_Kuhic@sydney.com", comment.Email);
                LogPass($"Test {nameof(Getcomments_ShouldReturn200)} completed successfully");
            }
            catch (Exception ex)
            {
                LogFail($"Test {nameof(Getcomments_ShouldReturn200)} failed: {ex.Message}");
                throw;
            }
            finally
            {
                ExtentManager.Flush();
            }
        }

        private void ValidateEmailFormat(string email)
        {
            try
            {
                LogInfo($"Validating email format: {email}");
                
                if (string.IsNullOrEmpty(email))
                {
                    LogFail("Email is null or empty");
                    throw new FormatException("Email cannot be null or empty");
                }

                // Email validation pattern
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                bool isEmailValid = Regex.IsMatch(email, emailPattern);

                if (isEmailValid)
                {
                    LogPass($"Email '{email}' is valid");
                }
                else
                {
                    LogFail($"Email '{email}' is invalid. Pattern: {emailPattern}");
                    throw new FormatException($"Invalid email format: {email}");
                }
            }
            catch (Exception ex)
            {
                LogFail($"Email validation failed: {ex.Message}");
                throw;
            }
        }

        /*[Fact]
        public async Task GetPost_ShouldMatchSchema()
        {
            // Get response from API
            var response = await _postPage.GetPostAsync(1);
            Assert.Equal(200, response.Status);

            // Parse response body
            string responseBody = await response.TextAsync();

            // Load schema from file
            string schemaJson = File.ReadAllText("schemas/post_schema.json");

            // Validate response against schema
            bool isValid = JsonSchemaValidator.Validate(responseBody, schemaJson, out string validationErrors);

            // Assert schema validation passes
            Assert.True(isValid, $"Schema validation failed: {validationErrors}");

            // Log validation errors if any
            if (!isValid)
            {
                Log.Error($"Schema validation failed for post ID 1. Errors: {validationErrors}");
            }
        }
        */
    }

    // Response model for comments
    public class CommentResponse
    {
        public int PostId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}

