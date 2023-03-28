using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.ImageGeneration.OpenAI;

public record OpenAIImageData
{
    [JsonPropertyName("url")]
    public string Url { get; init; } = "";
}
