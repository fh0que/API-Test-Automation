using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlaywrightTests.Utilities;

public class TestDataLoader
{
    private static readonly string TestDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "test", "TestData");
    
    public static async Task<TestData> LoadTestDataAsync(string environment = "development")
    {
        string filePath = Path.Combine(TestDataPath, $"{environment.Trim().ToLower()}.json");
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Test data file not found for environment: {environment} at path: {filePath}");
        }

        string jsonContent = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<TestData>(jsonContent) ?? 
            throw new JsonException($"Failed to deserialize test data for environment: {environment}");
    }
}

public class TestData
{
    [JsonPropertyName("baseUrl")]
    public string BaseUrl { get; set; } = string.Empty;

    [JsonPropertyName("testData")]
    public TestDataContent TestDataContent { get; set; } = new();
}

public class TestDataContent
{
    [JsonPropertyName("posts")]
    public PostTestData Posts { get; set; } = new();

    [JsonPropertyName("comments")]
    public CommentTestData Comments { get; set; } = new();
}

public class PostTestData
{
    [JsonPropertyName("validPost")]
    public PostData ValidPost { get; set; } = new();

    [JsonPropertyName("invalidPost")]
    public PostData InvalidPost { get; set; } = new();
}

public class CommentTestData
{
    [JsonPropertyName("validComment")]
    public CommentData ValidComment { get; set; } = new();

    [JsonPropertyName("invalidComment")]
    public CommentData InvalidComment { get; set; } = new();
}

public class PostData
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("userId")]
    public int UserId { get; set; }
}

public class CommentData
{
    [JsonPropertyName("postId")]
    public int PostId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;
} 