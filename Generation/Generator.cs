using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RandomImageGenerator.Corpora;
using RandomImageGenerator.ImageGeneration;
using RandomImageGenerator.SafeList;
using RandomImageGenerator.Storage;
using RandomImageGenerator.TextGeneration;

namespace RandomImageGenerator.Generation;

public class Generator : IGenerator
{
    private readonly IList<byte[]> _safeAddresses;
    private readonly ISentenceGeneratorFactory _sentenceGeneratorFactory;
    private readonly DeepAIGenerator _imageGenerator;
    private readonly IStorageHandler _storageHandler;
    private readonly ILogger<Generator> _logger;

    public Generator(
        IOptions<SafeListOptions> safeListOptions,
        ISentenceGeneratorFactory sentenceGeneratorFactory,
        DeepAIGenerator imageGenerator,
        IStorageHandler storageHandler,
        ILogger<Generator> log)
    {
        _safeAddresses = safeListOptions.Value.Addresses.Select(address => IPAddress.Parse(address).GetAddressBytes()).ToList();
        _sentenceGeneratorFactory = sentenceGeneratorFactory;
        _imageGenerator = imageGenerator;
        _storageHandler = storageHandler;
        _logger = log;
    }

    public async Task<ImageGenerationResult> GenerateImage(Corpus corpus, IPAddress? address, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP GenerateImage trigger function processed a request.");
        if (!IsValidIp(address))
        {
            _logger.LogInformation("GenerateImage blocked due to untrusted IP");
            return ImageGenerationResult.AccessDenied;
        }

        var sentence = GenerateSentence(corpus);
        var image = await _imageGenerator.Generate(sentence, cancellationToken);
        if (image.Length == 0)
            return ImageGenerationResult.ImageGenerationFailed;
        else
            return ImageGenerationResult.Success(image, sentence);
    }

    public async Task<LinkGenerationResult> GenerateImageLink(Corpus corpus, IPAddress? address, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP GenerateImageLink trigger function processed a request.");
        if (!IsValidIp(address))
        {
            _logger.LogInformation("GenerateImageLink blocked due to untrusted IP");
            return LinkGenerationResult.AccessDenied;
        }

        var sentence = GenerateSentence(corpus);
        var image = await _imageGenerator.Generate(sentence, cancellationToken);
        if (image.Length == 0)
            return LinkGenerationResult.ImageGenerationFailed;

        var link = await _storageHandler.StoreImage(image, cancellationToken);
        return LinkGenerationResult.Success(link, sentence);
    }

    private bool IsValidIp(IPAddress? address)
    {
        var ip = address?.GetAddressBytes();
        return ip == null || _safeAddresses.Any(safe => safe.SequenceEqual(ip));
    }

    private string GenerateSentence(Corpus corpus)
    {
        var corpusPath = CorpusSource.GetCorpusPath(corpus);
        var generator = _sentenceGeneratorFactory.CreateGenerator(corpusPath);
        return generator.Generate();
    }
}
