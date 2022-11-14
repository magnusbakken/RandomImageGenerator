using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using RandomImageGenerator.Generation;

namespace RandomImageGenerator;

public class GenerateImageTrigger
{
    private readonly IGenerator _generator;

    public GenerateImageTrigger(IGenerator generator) => _generator = generator;

    [Function("GenerateImage")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        var ipAddress = GetClientIpAddress(req);
        return await _generator.Generate(ipAddress, cancellationToken) switch
        {
            GeneratorResult.AccessDeniedResult => req.CreateResponse(HttpStatusCode.Forbidden),
            GeneratorResult.ImageGenerationFailedResult => await WriteResponse(req.CreateResponse(HttpStatusCode.BadRequest), "Unable to generate image"),
            GeneratorResult.SuccessResult r => await Success(req, r),
            _ => throw new InvalidOperationException("Unknown generator result type")
        };
    }

    private static async Task<HttpResponseData> Success(HttpRequestData req, GeneratorResult.SuccessResult r)
    {
        var filename = $"{r.Sentence}.jpg";
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
}
