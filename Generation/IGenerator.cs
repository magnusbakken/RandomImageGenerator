using System.Net;
using RandomImageGenerator.Corpora;
using RandomImageGenerator.ImageGeneration;
using RandomImageGenerator.TextGeneration;

namespace RandomImageGenerator.Generation;

public interface IGenerator
{
    Task<ImageGenerationResult> GenerateImage(
        TextGeneratorType textGeneratorType,
        ImageGeneratorType imageGeneratorType,
        Corpus corpus,
        IPAddress? address,
        CancellationToken cancellationToken);

    Task<LinkGenerationResult> GenerateImageLink(
        TextGeneratorType textGeneratorType,
        ImageGeneratorType imageGeneratorType,
        Corpus corpus,
        IPAddress? address,
        CancellationToken cancellationToken);
}