using System.Net;
using RandomImageGenerator.Corpora;
using RandomImageGenerator.ImageGeneration;

namespace RandomImageGenerator.Generation;

public interface IGenerator
{
    Task<ImageGenerationResult> GenerateImage(
        ImageGeneratorType imageGeneratorType,
        Corpus corpus,
        IPAddress? address,
        CancellationToken cancellationToken);

    Task<LinkGenerationResult> GenerateImageLink(
        ImageGeneratorType imageGeneratorType,
        Corpus corpus,
        IPAddress? address,
        CancellationToken cancellationToken);
}