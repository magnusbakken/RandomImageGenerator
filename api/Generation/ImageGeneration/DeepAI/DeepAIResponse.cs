using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.ImageGeneration.DeepAI;

public record DeepAIResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("output_url")]
    public string OutputUrl { get; init; } = "";
}
