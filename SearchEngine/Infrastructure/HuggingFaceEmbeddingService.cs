using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SearchEngine.Infrastructure;

public sealed class HuggingFaceEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public HuggingFaceEmbeddingService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["HuggingApiKey"];
    }

    public async Task<float[]> GenerateEmbeddingAsync(string inputText)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api-inference.huggingface.co/pipeline/feature-extraction/sentence-transformers/all-MiniLM-L6-v2")
        {
            Content = new StringContent(JsonSerializer.Serialize(new { inputs = inputText }), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<List<float>>>(responseContent);

        return result?.FirstOrDefault()?.ToArray() ?? Array.Empty<float>();
    }
}