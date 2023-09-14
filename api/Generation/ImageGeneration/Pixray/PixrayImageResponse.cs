using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.ImageGeneration.Pixray;

public record PixrayImageResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";
}