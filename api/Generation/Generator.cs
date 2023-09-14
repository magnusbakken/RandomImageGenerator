using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RandomImageGenerator.Corpora;
using RandomImageGenerator.Generation.ImageGeneration;
using RandomImageGenerator.Generation.TextGeneration;
using RandomImageGenerator.SafeList;
using RandomImageGenerator.Storage;

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

    public async Task<GenerationResult<ImageGenerationData>> GenerateImage(GeneratorOptions options, IPAddress? address, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP GenerateImage trigger function processed a request.");
        if (!IsValidIp(address))
        {
            _logger.LogInformation("GenerateImage blocked due to untrusted IP");
            return GenerationResult<ImageGenerationData>.AccessDenied;
        }

        var sentence = await GenerateSentence(options.TextGeneratorType, options.Corpus, cancellationToken);
        if (sentence == null)
            return GenerationResult<ImageGenerationData>.SentenceGenerationFailed;

        var imageGenerator = _imageGeneratorFactory.Create(options.ImageGeneratorType);
        var image = await imageGenerator.Generate(sentence, cancellationToken);
        if (image.Length == 0)
            return GenerationResult<ImageGenerationData>.ImageGenerationFailed;
        else
            return GenerationResult<ImageGenerationData>.Success(new(image, sentence));
    }

    public async Task<GenerationResult<LinkGenerationData>> GenerateImageLink(GeneratorOptions options, IPAddress? address, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP GenerateImageLink trigger function processed a request.");
        if (!IsValidIp(address))
        {
            _logger.LogInformation("GenerateImageLink blocked due to untrusted IP: {0}", address);
            return GenerationResult<LinkGenerationData>.AccessDenied;
        }

        var sentence = await GenerateSentence(options.TextGeneratorType, options.Corpus, cancellationToken);
        if (sentence == null)
            return GenerationResult<LinkGenerationData>.SentenceGenerationFailed;

        var imageGenerator = _imageGeneratorFactory.Create(options.ImageGeneratorType);
        var image = await imageGenerator.Generate(sentence, cancellationToken);
        if (image.Length == 0)
            return GenerationResult<LinkGenerationData>.ImageGenerationFailed;

        var link = await _storageHandler.StoreImage(image, cancellationToken);
        return GenerationResult<LinkGenerationData>.Success(new(link, sentence));
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
