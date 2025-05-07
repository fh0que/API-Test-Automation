using Xunit;
using Allure.Net.Commons;
using Allure.Xunit.Attributes;
using PlaywrightTests.Utilities;
using System;
using System.Threading.Tasks;
using PlaywrightTests.PageObjects;
namespace PlaywrightTests.Tests
{
    public class SampleTest : TBase
    {
        private readonly PostPage _postPage;

        public SampleTest()
        {
            _postPage = new PostPage(ApiClient);
        }

        [Fact]
        public async Task SampleAPITest2()
        {
            try
            {   
                Test = ExtentManager.CreateTest(nameof(SampleAPITest2));
                LogInfo($"Starting test: {nameof(SampleAPITest2)}");

                // Test DELETE request
                var deleteResponse = await ApiClient.DeleteAsync("posts/1");
                LogRequest("DELETE", "posts/1", null);
                LogResponse(deleteResponse.Status, await deleteResponse.TextAsync());

                // Test GET request
                var getResponse = await ApiClient.GetAsync("posts/1");
                LogRequest("GET", "posts/1", null);
                LogResponse(getResponse.Status, await getResponse.TextAsync());

                // Test PUT request
                var putData = new { title = "updated title", body = "updated body", userId = 1 };
                var putResponse = await ApiClient.PutAsync("posts/1", putData);
                LogRequest("PUT", "posts/1", System.Text.Json.JsonSerializer.Serialize(putData));
                LogResponse(putResponse.Status, await putResponse.TextAsync());

                // Test POST request
                var postData = new { title = "foo", body = "bar", userId = 1 };
                var postResponse = await ApiClient.PostAsync("posts", postData);
                LogRequest("POST", "posts", System.Text.Json.JsonSerializer.Serialize(postData));
                LogResponse(postResponse.Status, await postResponse.TextAsync());

                LogPass($"Test {nameof(SampleAPITest2)} completed successfully");
            }
            catch (Exception ex)
            {
                LogFail($"Test {nameof(SampleAPITest2)} failed: {ex.Message}");
                throw;
            }
        }
    }
}
