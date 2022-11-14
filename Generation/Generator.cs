using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RandomImageGenerator.Corpora;
using RandomImageGenerator.ImageGeneration;
using RandomImageGenerator.SafeList;
using RandomImageGenerator.TextGeneration;

namespace RandomImageGenerator.Generation;

public class Generator : IGenerator
{
    private readonly IList<byte[]> _safeAddresses;
    private readonly ISentenceGeneratorFactory _sentenceGeneratorFactory;
    private readonly DeepAIGenerator _imageGenerator;
    private readonly ILogger<Generator> _logger;

    public Generator(
        IOptions<SafeListOptions> safeListOptions,
        ISentenceGeneratorFactory sentenceGeneratorFactory,
        DeepAIGenerator imageGenerator,
        ILogger<Generator> log)
    {
        _safeAddresses = safeListOptions.Value.Addresses.Select(address => IPAddress.Parse(address).GetAddressBytes()).ToList();
        _sentenceGeneratorFactory = sentenceGeneratorFactory;
        _imageGenerator = imageGenerator;
        _logger = log;
    }

    public async Task<GeneratorResult> Generate(IPAddress? address, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var ip = address?.GetAddressBytes();
        if (ip != null && !_safeAddresses.Any(safe => safe.SequenceEqual(ip)))
        {
            _logger.LogInformation("Blocked due to untrusted IP");
            return GeneratorResult.AccessDenied;
        }

        var corpus = CorpusSource.GetCorpusPath(Corpus.EngNews202010K);
        var generator = _sentenceGeneratorFactory.CreateGenerator(corpus);
        var sentence = generator.Generate();

        var image = await _imageGenerator.Generate(sentence, cancellationToken);
        if (image.Length == 0)
            return GeneratorResult.ImageGenerationFailed;
        else
            return GeneratorResult.Success(image, sentence);
    }
}
