using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.TextGeneration.OpenAI;

public record OpenAICompletionResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("object")]
    public string Object { get; init; } = "";

    [JsonPropertyName("created")]
    public int Created { get; init; }

    [JsonPropertyName("model")]
    public string Model { get; init; } = "";

    [JsonPropertyName("choices")]
    public IList<OpenAICompletionChoice> Choices { get; init; } = new List<OpenAICompletionChoice>();

    [JsonPropertyName("usage")]
    public OpenAICompletionUsage Usage { get; init; } = new();
}