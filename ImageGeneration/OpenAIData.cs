using System.Text.Json.Serialization;

namespace RandomImageGenerator.ImageGeneration;

public record OpenAIData
{
    [JsonPropertyName("url")]
    public string Url { get; init; } = "";
}
