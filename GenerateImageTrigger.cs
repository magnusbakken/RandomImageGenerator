using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using RandomImageGenerator.Corpora;
using RandomImageGenerator.Generation;

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
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            var corpus = GetCorpusFromQuery(query);
            var ipAddress = GetClientIpAddress(req);
            return await _generator.Generate(corpus, ipAddress, cancellationToken) switch
            {
                GeneratorResult.AccessDeniedResult => req.CreateResponse(HttpStatusCode.Forbidden),
                GeneratorResult.ImageGenerationFailedResult => await WriteResponse(req.CreateResponse(HttpStatusCode.BadRequest), "Unable to generate image"),
                GeneratorResult.SuccessResult r => await Success(req, r),
                _ => throw new InvalidOperationException("Unknown generator result type")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            throw;
        }
    }

    private static async Task<HttpResponseData> Success(HttpRequestData req, GeneratorResult.SuccessResult r)
    {
        var sanitizedSentence = SanitizeFilename(r.Sentence);
        var filename = $"{(sanitizedSentence.EndsWith('.') ? sanitizedSentence : (sanitizedSentence + "."))}jpg";
        return await WriteResponse(SetHeader(req.CreateResponse(HttpStatusCode.OK), "Content-Disposition", $"attachment; filename=\"{filename}\""), r.Image);
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

    private static Corpus GetCorpusFromQuery(NameValueCollection query)
    {
        return Enum.TryParse<Corpus>(query.Get("corpus"), ignoreCase: true, out var corpus) ? corpus : Corpus.EngNews202010K;
    }
}
