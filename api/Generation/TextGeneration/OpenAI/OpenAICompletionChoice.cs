using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.TextGeneration.OpenAI;

public record OpenAICompletionChoice
{
    [JsonPropertyName("text")]
    public string Text { get; init; } = "";

    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("model")]
    public string Model { get; init; } = "";

    [JsonPropertyName("logprobs")]
    public IList<string> LogProbs { get; init; } = new List<string>();

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; init; } = "";
}