
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RandomImageGenerator.Generation.ImageGeneration.Picsart;

public class PicsartGenerator : IImageGenerator
{
    private const string BaseUrl = "https://genai-api.picsart.io";
    private const string ImageEndpointUrl = "/v1/text2image";
    private const string InferenceEndpointUrl = "/v1/text2image/inferences/{0}";
    private const string ApiKeyHeaderName = "X-Picsart-API-Key";

    private readonly PicsartOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger<PicsartGenerator> _logger;

    public PicsartGenerator(
        IHttpClientFactory httpClientFactory,
        IOptions<PicsartOptions> options,
        ILogger<PicsartGenerator> log)
    {
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient(nameof(PicsartGenerator));
        _httpClient.DefaultRequestHeaders.Add(ApiKeyHeaderName, _options.ApiKey);
        _logger = log;
    }

    public async Task<byte[]> Generate(string sentence, CancellationToken cancellationToken)
    {
        var request = new PicsartImageRequest()
        {
            Count = 1,
            Height = _options.Height,
            NegativePrompt = _options.NegativePrompt,
            Prompt = sentence,
            Width = _options.Width,
        };

        var response = await _httpClient.PostAsJsonAsync(ImageEndpointUrl, request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"Successfully generated image for sentence: {{0}}", sentence);
            var content = await response.Content.ReadFromJsonAsync<PicsartImageResponse>(cancellationToken: cancellationToken);
            if (content != null)
            {
                var image = await GetImageFromInference(content.InferenceId, cancellationToken);
                if (image != null)
                    return image;
            }
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation($"Got unsuccessful image generation response from Picsart: [{{0}}]: {{1}}", response.StatusCode, content);
        }

        _logger.LogWarning($"Failed to generate image for sentence: {{0}}", sentence);
        return Array.Empty<byte>();
    }

    private async Task<byte[]?> GetImageFromInference(string inferenceId, CancellationToken cancellationToken)
    {
        var inferenceUrl = string.Format(InferenceEndpointUrl, inferenceId);
        var inferenceResponse = await _httpClient.GetAsync(inferenceUrl, cancellationToken);
        if (inferenceResponse.IsSuccessStatusCode)
        {
            var inferenceJson = await inferenceResponse.Content.ReadFromJsonAsync<PicsartInferenceResponse>(cancellationToken: cancellationToken);
            if (inferenceJson?.Data?.FirstOrDefault() is PicsartImage firstImage)
            {
                _logger.LogInformation($"Downloading image from {{0}}", firstImage.Url);
                return await _httpClient.GetByteArrayAsync(firstImage.Url, cancellationToken);
            }

            var invalidContent = await inferenceResponse.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Unable to get Picsart image from response: {0}", invalidContent);
        }
        else
        {
            var errorContent = await inferenceResponse.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation($"Got unsuccessful response from Picsart: [{{0}}]: {{1}}", inferenceResponse.StatusCode, errorContent);
        }

        return null;
    }
}