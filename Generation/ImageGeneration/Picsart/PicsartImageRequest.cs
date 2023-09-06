using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.ImageGeneration.Picsart;

public record PicsartImageRequest
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; init; } = "";

    [JsonPropertyName("negative_prompt")]
    public string NegativePrompt { get; init; } = "";

    [JsonPropertyName("width")]
    public int Width { get; init; } = 1024;

    [JsonPropertyName("height")]
    public int Height { get; init; } = 1024;

    [JsonPropertyName("count")]
    public int Count { get; init; } = 1;
}