using System.Text.Json.Serialization;

namespace RandomImageGenerator.ImageGeneration;

public record DeepAIResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("output_url")]
    public string OutputUrl { get; init; } = "";
}
