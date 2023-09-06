using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.ImageGeneration.Picsart;

public record PicsartInferenceResponse
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = "";

    [JsonPropertyName("data")]
    public IList<PicsartImage> Data { get; init; } = new List<PicsartImage>();
}