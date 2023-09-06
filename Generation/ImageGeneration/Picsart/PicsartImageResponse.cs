using System.Text.Json.Serialization;

namespace RandomImageGenerator.Generation.ImageGeneration.Picsart;

public record PicsartImageResponse
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = "";

    [JsonPropertyName("inference_id")]
    public string InferenceId { get; init; } = "";
}