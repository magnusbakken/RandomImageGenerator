using Microsoft.Extensions.Logging;

namespace RandomImageGenerator.Generation.ImageGeneration.Pixray;

public class PixrayImageDownloader
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PixrayImageDownloader> _logger;

    public PixrayImageDownloader(IHttpClientFactory httpClientFactory, ILogger<PixrayImageDownloader> log)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(PixrayImageDownloader));
        _logger = log;
    }

    public async Task<byte[]> Download(string url, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading image from {0}", url);
        return await _httpClient.GetByteArrayAsync(url, cancellationToken);
    }
}