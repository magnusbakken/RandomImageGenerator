using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RandomImageGenerator.ImageGeneration;

public class DeepAIGenerator : IImageGenerator
{
    private const string Url = "https://api.deepai.org/api/text2img";
    private readonly DeepAIOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger<DeepAIGenerator> _logger;

    public DeepAIGenerator(IHttpClientFactory httpClientFactory, IOptions<DeepAIOptions> options, ILogger<DeepAIGenerator> log)
    {
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient(nameof(DeepAIGenerator));
        _httpClient.DefaultRequestHeaders.Add("api-key", _options.ApiKey);
        _logger = log;
    }

    public async Task<byte[]> Generate(string sentence, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating image for sentence: {0}", sentence);
        var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("text", sentence) });
        var response = await _httpClient.PostAsync(Url, formContent, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully generated image for sentence: {0}", sentence);
            var content = await response.Content.ReadFromJsonAsync<DeepAIResponse>(cancellationToken: cancellationToken);
            if (content != null)
            {
                _logger.LogInformation("Downloading image from {0}", content.OutputUrl);
                return await _httpClient.GetByteArrayAsync(content.OutputUrl, cancellationToken);
            }
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Got unsuccessful response from DeepAI: [{0}]: {1}", response.StatusCode, content);
        }

        _logger.LogInformation("Failed to generate image for sentence: {0}", sentence);
        return Array.Empty<byte>();
    }
}
