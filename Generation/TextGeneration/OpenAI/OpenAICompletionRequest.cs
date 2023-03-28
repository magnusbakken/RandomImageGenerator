using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.TextGeneration.OpenAI;

public record OpenAICompletionRequest
{
    [JsonPropertyName("model")]
    public string Model { get; init; } = "";

    [JsonPropertyName("prompt")]
    public string Prompt { get; init; } = "";

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; init; } = 16;

    [JsonPropertyName("temperature")]
    public double Temperature { get; init; }

    [JsonPropertyName("user")]
    public string? User { get; init; }
}