using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.ImageGeneration.Pixray;

public record PixrayPredictionResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("output")]
    public IList<string> Output { get; init; } = new List<string>();

    [JsonPropertyName("status")]
    public string Status { get; init; } = PixrayPredictionStatus.Starting;
}