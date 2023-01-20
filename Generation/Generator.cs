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
    private readonly IImageGeneratorFactory _imageGeneratorFactory;
    private readonly IStorageHandler _storageHandler;
    private readonly ILogger<Generator> _logger;

    public Generator(
        IOptions<SafeListOptions> safeListOptions,
        ISentenceGeneratorFactory sentenceGeneratorFactory,
        IImageGeneratorFactory imageGeneratorFactory,
        IStorageHandler storageHandler,
        ILogger<Generator> log)
    {
        _safeAddresses = safeListOptions.Value.Addresses.Select(address => IPAddress.Parse(address).GetAddressBytes()).ToList();
        _sentenceGeneratorFactory = sentenceGeneratorFactory;
        _imageGeneratorFactory = imageGeneratorFactory;
        _storageHandler = storageHandler;
        _logger = log;
    }

    public async Task<ImageGenerationResult> GenerateImage(
        TextGeneratorType textGeneratorType,
        ImageGeneratorType imageGeneratorType,
        Corpus corpus,
        IPAddress? address,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP GenerateImage trigger function processed a request.");
        if (!IsValidIp(address))
        {
            _logger.LogInformation("GenerateImage blocked due to untrusted IP");
            return ImageGenerationResult.AccessDenied;
        }

        var sentence = await GenerateSentence(textGeneratorType, corpus, cancellationToken);
        if (sentence == null)
            return ImageGenerationResult.SentenceGenerationFailed;

        var imageGenerator = _imageGeneratorFactory.Create(imageGeneratorType);
        var image = await imageGenerator.Generate(sentence, cancellationToken);
        if (image.Length == 0)
            return ImageGenerationResult.ImageGenerationFailed;
        else
            return ImageGenerationResult.Success(image, sentence);
    }

    public async Task<LinkGenerationResult> GenerateImageLink(
        TextGeneratorType textGeneratorType,
        ImageGeneratorType imageGeneratorType,
        Corpus corpus,
        IPAddress? address,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP GenerateImageLink trigger function processed a request.");
        if (!IsValidIp(address))
        {
            _logger.LogInformation("GenerateImageLink blocked due to untrusted IP: {0}", address);
            return LinkGenerationResult.AccessDenied;
        }

        var sentence = await GenerateSentence(textGeneratorType, corpus, cancellationToken);
        if (sentence == null)
            return LinkGenerationResult.SentenceGenerationFailed;

        var imageGenerator = _imageGeneratorFactory.Create(imageGeneratorType);
        var image = await imageGenerator.Generate(sentence, cancellationToken);
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

    private async Task<string?> GenerateSentence(TextGeneratorType textGeneratorType, Corpus corpus, CancellationToken cancellationToken)
    {
        var corpusPath = CorpusSource.GetCorpusPath(corpus);
        var options = new SentenceGeneratorOptions() { MarkovCorpusPath = corpusPath };
        var generator = _sentenceGeneratorFactory.CreateGenerator(textGeneratorType, options);
        return await generator.Generate(cancellationToken);
    }
}
