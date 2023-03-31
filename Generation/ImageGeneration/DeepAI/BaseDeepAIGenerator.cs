using System.Net.Http.Json;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RandomImageGenerator.Generation.ImageGeneration.DeepAI;

public abstract class BaseDeepAIGenerator : IImageGenerator
{
    private readonly DeepAIOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger<BaseDeepAIGenerator> _logger;

    protected BaseDeepAIGenerator(IHttpClientFactory httpClientFactory, IOptions<DeepAIOptions> options, ILogger<BaseDeepAIGenerator> log)
    {
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient(GetType().Name);
        _httpClient.DefaultRequestHeaders.Add("api-key", _options.ApiKey);
        _logger = log;
    }

    public abstract string Url { get; }

    public abstract string GeneratorName { get; }

    public async Task<byte[]> Generate(string sentence, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Generating image for sentence with {GeneratorName}: {{0}}", sentence);
        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("text", sentence),
            new KeyValuePair<string, string>("grid_size", "1"),
            new KeyValuePair<string, string>("width", "512"),
            new KeyValuePair<string, string>("height", "512"),
        });

        var response = await _httpClient.PostAsync(Url, formContent, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"Successfully generated image for sentence with {GeneratorName}: {{0}}", sentence);
            var content = await response.Content.ReadFromJsonAsync<DeepAIResponse>(cancellationToken: cancellationToken);
            if (content != null)
            {
                _logger.LogInformation($"Downloading image with {GeneratorName} from {{0}}", content.OutputUrl);
                return await _httpClient.GetByteArrayAsync(content.OutputUrl, cancellationToken);
            }
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation($"Got unsuccessful response with {GeneratorName}: [{{0}}]: {{1}}", response.StatusCode, content);
        }

        _logger.LogWarning($"Failed to generate image with {GeneratorName} for sentence: {{0}}", sentence);
        return Array.Empty<byte>();
    }
}
