using System.Text.Json.Serialization;

namespace RandomImageGenerator.ImageGeneration.OpenAI;

public record OpenAIImageRequest
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; init; } = "";

    [JsonPropertyName("user")]
    public string? User { get; init; }
}
