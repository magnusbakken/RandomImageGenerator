using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RandomImageGenerator.Shared.OpenAI;

namespace RandomImageGenerator.Generation.ImageGeneration.OpenAI;

public class OpenAIImageGenerator : IImageGenerator
{
    private const string Url = OpenAIConstants.BaseUrl + "/v1/images/generations";

    private readonly OpenAIOptions _options;
    private readonly HttpClient _httpClient;
    private readonly OpenAIImageDownloader _downloader;
    private readonly ILogger<OpenAIImageGenerator> _logger;

    public OpenAIImageGenerator(
        IHttpClientFactory httpClientFactory,
        IOptions<OpenAIOptions> options,
        OpenAIImageDownloader downloader,
        ILogger<OpenAIImageGenerator> log)
    {
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient(nameof(OpenAIImageGenerator));
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ApiKey}");
        _downloader = downloader;
        _logger = log;
    }

    public async Task<byte[]> Generate(string sentence, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating image for sentence: {0}", sentence);
        var jsonContent = JsonContent.Create(new OpenAIImageRequest()
        {
            Prompt = sentence,
            User = OpenAIConstants.OpenAIUser,
        });

        var response = await _httpClient.PostAsync(Url, jsonContent, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully generated image for sentence: {0}", sentence);
            var content = await response.Content.ReadFromJsonAsync<OpenAIImageResponse>(cancellationToken: cancellationToken);
            var url = content?.Data?[0]?.Url;
            if (url == null)
                _logger.LogWarning("Unable to extract URL from response {0}", content);
            else
                return await _downloader.Download(url, cancellationToken);
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Got unsuccessful response from OpenAI: [{0}]: {1}", response.StatusCode, content);
        }

        _logger.LogWarning("Failed to generate image for sentence: {0}", sentence);
        return Array.Empty<byte>();
    }
}
