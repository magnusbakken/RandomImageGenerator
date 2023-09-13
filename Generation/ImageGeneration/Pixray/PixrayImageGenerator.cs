
using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RandomImageGenerator.Generation.ImageGeneration.Pixray;

public class PixrayImageGenerator : IImageGenerator
{
    private const string BaseUrl = "https://api.replicate.com";
    private const string PredictionsUrl = BaseUrl + "/v1/predictions";
    private const string SinglePredictionUrl = BaseUrl + "/v1/predictions/{0}";
    private const string SinglePredictionCancelUrl = BaseUrl + "/v1/predictions/{0}/cancel";

    private readonly PixrayOptions _options;
    private readonly HttpClient _httpClient;
    private readonly PixrayImageDownloader _downloader;
    private readonly ILogger<PixrayImageGenerator> _logger;

    public PixrayImageGenerator(
        IHttpClientFactory httpClientFactory,
        IOptions<PixrayOptions> options,
        PixrayImageDownloader downloader,
        ILogger<PixrayImageGenerator> log)
    {
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient(nameof(PixrayImageGenerator));
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {_options.ApiKey}");
        _downloader = downloader;
        _logger = log;
    }

    public async Task<byte[]> Generate(string sentence, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating image for sentence: {0}", sentence);
        var input = new PixrayImageRequest()
        {
            Version = _options.ApiVersion,
            Input = new Dictionary<string, object>()
            {
                ["prompts"] = sentence,
                ["aspect"] = _options.Aspect,
                ["drawer"] = _options.Drawer,
                ["quality"] = _options.Quality,
                ["scale"] = _options.Scale,
            }
        };

        using var response = await _httpClient.PostAsJsonAsync(PredictionsUrl, input, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully generated image for sentence: {0}", sentence);
            var content = await response.Content.ReadFromJsonAsync<PixrayImageResponse>(cancellationToken: cancellationToken);
            if (content == null)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Unable to parse response from Pixray: {0}", errorContent);
            }
            else
            {
                return await Poll(content.Id, pollCount: 1, cancellationToken: cancellationToken) ?? Array.Empty<byte>();
            }
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Got unsuccessful response from Pixray: [{0}]: {1}", response.StatusCode, content);
        }

        _logger.LogWarning("Failed to generate image for sentence: {0}", sentence);
        return Array.Empty<byte>();
    }

    public async Task<byte[]?> Poll(string id, int pollCount, CancellationToken cancellationToken)
    {
        var url = string.Format(CultureInfo.InvariantCulture, SinglePredictionUrl, id);
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Got unsuccessful response from Pixray for ID {0}: [{1}]: {2}", id, response.StatusCode, errorContent);
            return null;
        }

        var content = await response.Content.ReadFromJsonAsync<PixrayPredictionResponse>(cancellationToken: cancellationToken);
        if (content == null)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Unable to read response from Pixray for ID {0} as JSON: [{1}]: {2}", id, response.StatusCode, errorContent);
            return null;
        }

        if (content.Status == PixrayPredictionStatus.Succeeded || pollCount >= _options.MaxPollCount)
        {
            return await Download(content, cancellationToken);
        }
        else if (content.Status == PixrayPredictionStatus.Processing || content.Status == PixrayPredictionStatus.Starting)
        {
            _logger.LogInformation("Pixray prediction with ID {0} still in progress (status {1}) after {2} polling iterations", content.Id, content.Status, pollCount);
            await Task.Delay(_options.PollDelayMs);
            return await Poll(id, pollCount + 1, cancellationToken);
        }
        else
        {
            _logger.LogWarning("Encountered unexpected Pixray prediction status {0} for ID {1}. Aborting...", content.Status, content.Id);
            return null;
        }
    }

    private async Task<byte[]?> Download(PixrayPredictionResponse content, CancellationToken cancellationToken)
    {
        var lastImage = content.Output.LastOrDefault();
        if (lastImage == null)
        {
            _logger.LogInformation("Pixray image generation succeeded (or timed out) for ID {0} but no image found", content.Id);
            return null;
        }

        var result = await _downloader.Download(lastImage, cancellationToken);

        if (content.Status != PixrayPredictionStatus.Succeeded)
            await Cancel(content.Id, cancellationToken);

        return result;
    }

    private async Task Cancel(string id, CancellationToken cancellationToken)
    {
        var url = string.Format(CultureInfo.InvariantCulture, SinglePredictionCancelUrl, id);
        using var response = await _httpClient.PostAsJsonAsync(url, "", cancellationToken);
        if (response.IsSuccessStatusCode)
            _logger.LogInformation("Successfully cancelled unfinished Pixray prediction with ID {0}", id);
        else
            _logger.LogWarning("Failed to cancel unfinished Pixray prediction with ID {0}", id);
    }
}