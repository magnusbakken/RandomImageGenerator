using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using RandomImageGenerator.Generation;
using RandomImageGenerator.Models;

namespace RandomImageGenerator;

public class GenerateImageTrigger
{
    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IGenerator _generator;
    private readonly ILogger<GenerateImageTrigger> _logger;

    public GenerateImageTrigger(IGenerator generator, ILogger<GenerateImageTrigger> logger)
    {
        _generator = generator;
        _logger = logger;
    }

    [Function("GenerateImage")]
    public async Task<HttpResponseData> GenerateImage(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        return await Generate<ImageGenerationData>(
            req,
            cancellationToken,
            nameof(GenerateImage),
            async (input, ipAddress) => await _generator.GenerateImage(input, ipAddress, cancellationToken),
            async data => await SendImage(req, data));
    }

    [Function("GenerateImageLink")]
    public async Task<HttpResponseData> GenerateImageLink(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        return await Generate<LinkGenerationData>(
            req,
            cancellationToken,
            nameof(GenerateImageLink),
            async (input, ipAddress) => await _generator.GenerateImageLink(input, ipAddress, cancellationToken),
            async data => await SendImageLink(req, data));
    }

    private async Task<HttpResponseData> Generate<T>(
        HttpRequestData req,
        CancellationToken cancellationToken,
        string operation,
        Func<GeneratorOptions, IPAddress?, Task<GenerationResult<T>>> action,
        Func<T, Task<HttpResponseData>> processor)
    {
        try
        {
            var input = await DeserializeGenerateImageRequest(req, cancellationToken);
            if (input == null)
                return await BadRequest(req, "Invalid JSON input");

            var ipAddress = GetClientIpAddress(req);
            var result = await action(CreateGeneratorOptions(input), ipAddress);
            return result switch
            {
                GenerationResult<T>.AccessDeniedResult => req.CreateResponse(HttpStatusCode.Forbidden),
                GenerationResult<T>.SentenceGenerationFailedResult => await BadRequest(req, "Unable to generate prompt for image"),
                GenerationResult<T>.ImageGenerationFailedResult => await BadRequest(req, "Unable to generate image"),
                GenerationResult<T>.SuccessResult r => await processor(r.Data),
                _ => throw new InvalidOperationException($"Unknown generator result type: {result}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unhandled exception occurred in {operation}");
            throw;
        }
    }

    private static async Task<GenerateImageRequest?> DeserializeGenerateImageRequest(HttpRequestData req, CancellationToken cancellationToken)
    {
        return await JsonSerializer.DeserializeAsync<GenerateImageRequest>(req.Body, SerializerOptions, cancellationToken);
    }

    private static async Task<HttpResponseData> BadRequest(HttpRequestData req, string text)
    {
        return await WriteResponse(req.CreateResponse(HttpStatusCode.BadRequest), text);
    }

    private static async Task<HttpResponseData> SendImage(HttpRequestData req, ImageGenerationData data)
    {
        var sanitizedSentence = SanitizeFilename(data.Sentence);
        var filename = $"{(sanitizedSentence.EndsWith('.') ? sanitizedSentence : (sanitizedSentence + "."))}jpg";
        return await WriteResponse(SetHeader(req.CreateResponse(HttpStatusCode.OK), "Content-Disposition", $"attachment; filename=\"{filename}\""), data.Image);
    }

    private static async Task<HttpResponseData> SendImageLink(HttpRequestData req, LinkGenerationData data)
    {
        var response = new GenerateImageLinkResponse { Link = data.Link, Sentence = data.Sentence };
        var json = JsonSerializer.Serialize(response, options: SerializerOptions);
        return await WriteResponse(req.CreateResponse(HttpStatusCode.OK), json);
    }

    private static async Task<HttpResponseData> WriteResponse(HttpResponseData response, string body)
    {
        await response.WriteStringAsync(body);
        return response;
    }

    private static async Task<HttpResponseData> WriteResponse(HttpResponseData response, byte[] body)
    {
        await response.WriteBytesAsync(body);
        return response;
    }

    private static HttpResponseData SetHeader(HttpResponseData response, string key, string value)
    {
        response.Headers.Add(key, value);
        return response;
    }

    private static IPAddress? GetClientIpAddress(HttpRequestData request)
    {
        if (!request.Headers.TryGetValues("X-Forwarded-For", out var values))
            return null;

        var ip = values.FirstOrDefault()?
            .Split(new char[] { ',' })?
            .FirstOrDefault()?
            .Split(new char[] { ':' })?
            .FirstOrDefault();
        return IPAddress.TryParse(ip, out var result) ? result : null;
    }

    private static string SanitizeFilename(string filename)
    {
        return Regex.Replace(Regex.Replace(filename, "[^\u0020-\u007E]", ""), "[\"/\\\\:]", "");
    }

    private static GeneratorOptions CreateGeneratorOptions(GenerateImageRequest request)
    {
        return new(request.TextGenerator, request.ImageGenerator, request.Corpus);
    }
}
