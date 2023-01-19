using System.Text.Json.Serialization;

namespace RandomImageGenerator.ImageGeneration.OpenAI;

public record OpenAIResponse
{
    [JsonPropertyName("created")]
    public int Created { get; init; }

    [JsonPropertyName("data")]
    public IList<OpenAIData> Data { get; init; } = new List<OpenAIData>();
}
