using System.Text.Json.Serialization;

namespace RandomImageGenerator.ImageGeneration.OpenAI;

public record OpenAIImageData
{
    [JsonPropertyName("url")]
    public string Url { get; init; } = "";
}
