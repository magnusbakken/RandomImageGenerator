using System.Collections.Specialized;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using RandomImageGenerator.Corpora;
using RandomImageGenerator.Generation;
using RandomImageGenerator.ImageGeneration;

namespace RandomImageGenerator;

public class GenerateImageTrigger
{
    private readonly IGenerator _generator;
    private readonly ILogger<GenerateImageTrigger> _logger;

    public GenerateImageTrigger(IGenerator generator, ILogger<GenerateImageTrigger> logger)
    {
        _generator = generator;
        _logger = logger;
    }

    [Function("GenerateImage")]
    public async Task<HttpResponseData> GenerateImage(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            var imageGeneratorType = GetImageGeneratorTypeFromQuery(query);
            var corpus = GetCorpusFromQuery(query);
            var ipAddress = GetClientIpAddress(req);
            return await _generator.GenerateImage(imageGeneratorType, corpus, ipAddress, cancellationToken) switch
            {
                ImageGenerationResult.AccessDeniedResult => req.CreateResponse(HttpStatusCode.Forbidden),
                ImageGenerationResult.ImageGenerationFailedResult => await WriteResponse(req.CreateResponse(HttpStatusCode.BadRequest), "Unable to generate image"),
                ImageGenerationResult.SuccessResult r => await SendImage(req, r),
                _ => throw new InvalidOperationException("Unknown generator result type")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred in GenerateImage");
            throw;
        }
    }

    [Function("GenerateImageLink")]
    public async Task<HttpResponseData> GenerateImageLink(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            var imageGeneratorType = GetImageGeneratorTypeFromQuery(query);
            var corpus = GetCorpusFromQuery(query);
            var ipAddress = GetClientIpAddress(req);
            return await _generator.GenerateImageLink(imageGeneratorType, corpus, ipAddress, cancellationToken) switch
            {
                LinkGenerationResult.AccessDeniedResult => req.CreateResponse(HttpStatusCode.Forbidden),
                LinkGenerationResult.ImageGenerationFailedResult => await WriteResponse(req.CreateResponse(HttpStatusCode.BadRequest), "Unable to generate image"),
                LinkGenerationResult.SuccessResult r => await SendImageLink(req, r),
                _ => throw new InvalidOperationException("Unknown generator result type")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred in GenerateImageLink");
            throw;
        }
    }

    private static async Task<HttpResponseData> SendImage(HttpRequestData req, ImageGenerationResult.SuccessResult r)
    {
        var sanitizedSentence = SanitizeFilename(r.Sentence);
        var filename = $"{(sanitizedSentence.EndsWith('.') ? sanitizedSentence : (sanitizedSentence + "."))}jpg";
        return await WriteResponse(SetHeader(req.CreateResponse(HttpStatusCode.OK), "Content-Disposition", $"attachment; filename=\"{filename}\""), r.Image);
    }

    private async Task<HttpResponseData> SendImageLink(
        HttpRequestData req,
        LinkGenerationResult.SuccessResult r)
    {
        var response = new { r.Link, r.Sentence };
        var json = JsonSerializer.Serialize(response);
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

    private static ImageGeneratorType GetImageGeneratorTypeFromQuery(NameValueCollection query)
    {
        return GetEnumFromQuery<ImageGeneratorType>(query, "imagegen", ImageGeneratorType.DeepAI);
    }

    private static Corpus GetCorpusFromQuery(NameValueCollection query)
    {
        return GetEnumFromQuery<Corpus>(query, "corpus", Corpus.EngNews202010K);
    }

    private static T GetEnumFromQuery<T>(NameValueCollection query, string queryKey, T defaultValue)
        where T : struct
    {
        return Enum.TryParse<T>(query.Get(queryKey), ignoreCase: true, out var corpus) ? corpus : defaultValue;
    }
}
