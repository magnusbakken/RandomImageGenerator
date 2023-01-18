using System.Text.Json.Serialization;

namespace RandomImageGenerator.ImageGeneration;

public record OpenAIRequest
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; init; } = "";

    [JsonPropertyName("user")]
    public string? User { get; init; }
}
