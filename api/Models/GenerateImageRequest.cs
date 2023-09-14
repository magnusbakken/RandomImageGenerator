using System.Text.Json.Serialization;
using RandomImageGenerator.Corpora;
using RandomImageGenerator.Generation.ImageGeneration;
using RandomImageGenerator.Generation.TextGeneration;

namespace RandomImageGenerator.Models;

public record GenerateImageRequest
{
    [JsonPropertyName("textgen")]
    public TextGeneratorType TextGenerator { get; init; }

    [JsonPropertyName("imagegen")]
    public ImageGeneratorType ImageGenerator { get; init; }

    [JsonPropertyName("corpus")]
    public Corpus Corpus { get; init; }
}
