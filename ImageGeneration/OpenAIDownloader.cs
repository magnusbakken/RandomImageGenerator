using Microsoft.Extensions.Logging;

namespace RandomImageGenerator.ImageGeneration;

public class OpenAIDownloader
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIDownloader> _logger;

    public OpenAIDownloader(IHttpClientFactory httpClientFactory, ILogger<OpenAIDownloader> log)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(OpenAIDownloader));
        _logger = log;
    }

    public async Task<byte[]> Download(string url, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading image from {0}", url);
        return await _httpClient.GetByteArrayAsync(url, cancellationToken);
    }
}
