using System.Text.Json.Serialization;

namespace RandomImageGenerator.Models;

public record GenerateImageLinkResponse
{
    [JsonPropertyName("link")]
    public string Link { get; init; } = "";

    [JsonPropertyName("sentence")]
    public string Sentence { get; init; } = "";
}
