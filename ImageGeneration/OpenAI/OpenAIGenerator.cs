using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RandomImageGenerator.ImageGeneration.OpenAI;

public class OpenAIGenerator : IImageGenerator
{
    private const string Url = "https://api.openai.com/v1/images/generations";
    private readonly OpenAIOptions _options;
    private readonly HttpClient _httpClient;
    private readonly OpenAIDownloader _downloader;
    private readonly ILogger<OpenAIGenerator> _logger;

    public OpenAIGenerator(
        IHttpClientFactory httpClientFactory,
        IOptions<OpenAIOptions> options,
        OpenAIDownloader downloader,
        ILogger<OpenAIGenerator> log)
    {
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient(nameof(OpenAIGenerator));
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ApiKey}");
        _downloader = downloader;
        _logger = log;
    }

    public async Task<byte[]> Generate(string sentence, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating image for sentence: {0}", sentence);
        var jsonContent = JsonContent.Create(new OpenAIRequest()
        {
            Prompt = sentence,
            User = "RandomImageGenerator Azure Functions",
        });

        var response = await _httpClient.PostAsync(Url, jsonContent, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully generated image for sentence: {0}", sentence);
            var content = await response.Content.ReadFromJsonAsync<OpenAIResponse>(cancellationToken: cancellationToken);
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
