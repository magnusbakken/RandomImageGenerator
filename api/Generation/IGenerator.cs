using System.Net;

namespace RandomImageGenerator.Generation;

public interface IGenerator
{
    Task<GenerationResult<ImageGenerationData>> GenerateImage(GeneratorOptions options, IPAddress? address, CancellationToken cancellationToken);

    Task<GenerationResult<LinkGenerationData>> GenerateImageLink(GeneratorOptions options, IPAddress? address, CancellationToken cancellationToken);
}