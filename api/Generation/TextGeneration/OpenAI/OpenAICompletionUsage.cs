using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.TextGeneration.OpenAI;

public record OpenAICompletionUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; init; }
}