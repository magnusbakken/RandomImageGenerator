using Microsoft.Extensions.Logging;

namespace RandomImageGenerator.Generation.ImageGeneration.OpenAI;

public class OpenAIImageDownloader
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIImageDownloader> _logger;

    public OpenAIImageDownloader(IHttpClientFactory httpClientFactory, ILogger<OpenAIImageDownloader> log)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(OpenAIImageDownloader));
        _logger = log;
    }

    public async Task<byte[]> Download(string url, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading image from {0}", url);
        return await _httpClient.GetByteArrayAsync(url, cancellationToken);
    }
}
