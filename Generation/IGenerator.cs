using System.Net;
using RandomImageGenerator.Corpora;

namespace RandomImageGenerator.Generation;

public interface IGenerator
{
    Task<ImageGenerationResult> GenerateImage(Corpus corpus, IPAddress? address, CancellationToken cancellationToken);

    Task<LinkGenerationResult> GenerateImageLink(Corpus corpus, IPAddress? address, CancellationToken cancellationToken);
}