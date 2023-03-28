using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.ImageGeneration.OpenAI;

public record OpenAIImageResponse
{
    [JsonPropertyName("created")]
    public int Created { get; init; }

    [JsonPropertyName("data")]
    public IList<OpenAIImageData> Data { get; init; } = new List<OpenAIImageData>();
}
