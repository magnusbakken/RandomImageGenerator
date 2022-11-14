using System.Net;

namespace RandomImageGenerator.Generation;

public interface IGenerator
{
    Task<GeneratorResult> Generate(IPAddress? address, CancellationToken cancellationToken);
}