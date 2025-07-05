namespace SearchEngine.Infrastructure;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string inputText);
}