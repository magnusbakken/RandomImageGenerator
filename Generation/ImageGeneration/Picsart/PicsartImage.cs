using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.ImageGeneration.Picsart;

public record PicsartImage
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("url")]
    public string Url { get; init; } = "";

    [JsonPropertyName("status")]
    public string Status { get; init; } = "";
}