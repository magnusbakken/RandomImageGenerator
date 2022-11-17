using System.Net;
using RandomImageGenerator.Corpora;

namespace RandomImageGenerator.Generation;

public interface IGenerator
{
    Task<GeneratorResult> Generate(Corpus corpus, IPAddress? address, CancellationToken cancellationToken);
}