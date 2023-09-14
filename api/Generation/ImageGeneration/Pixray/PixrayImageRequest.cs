using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.ImageGeneration.Pixray;

public record PixrayImageRequest
{
    [JsonPropertyName("version")]
    public string Version { get; init; } = "";

    [JsonPropertyName("input")]
    public IDictionary<string, object> Input { get; init; } = new Dictionary<string, object>();
}