using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RandomImageGenerator.Shared.OpenAI;

namespace RandomImageGenerator.TextGeneration.OpenAI;

public class OpenAISentenceGenerator : ISentenceGenerator
{
    private const string Url = OpenAIConstants.BaseUrl + "/v1/completions";

    private readonly OpenAIOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAISentenceGenerator> _logger;
    private readonly Random _random;

    public OpenAISentenceGenerator(
        IHttpClientFactory httpClientFactory,
        IOptions<OpenAIOptions> options,
        ILogger<OpenAISentenceGenerator> log)
    {
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient(nameof(OpenAISentenceGenerator));
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ApiKey}");
        _logger = log;

        _random = new Random();
    }

    public async Task<string?> Generate(CancellationToken cancellationToken)
    {
        var completionPrompt = ChooseRandomPrompt();
        _logger.LogInformation("Generating image prompt with completion prompt: {0}", completionPrompt);
        var jsonContent = JsonContent.Create(new OpenAICompletionRequest()
        {
            MaxTokens = _options.CompletionMaxTokens,
            Model = _options.CompletionModel,
            Prompt = completionPrompt,
            Temperature = _options.CompletionTemperature,
            User = OpenAIConstants.OpenAIUser,
        });

        var response = await _httpClient.PostAsync(Url, jsonContent, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully generated image prompt for completion prompt: {0}", completionPrompt);
            var content = await response.Content.ReadFromJsonAsync<OpenAICompletionResponse>(cancellationToken: cancellationToken);
            var imagePrompt = content?.Choices?[0]?.Text;
            if (imagePrompt == null)
                _logger.LogWarning("Unable to extract image prompt from response {0}", content);
            else
                return ProcessImagePrompt(imagePrompt);
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Got unsuccessful response from OpenAI: [{0}]: {1}", response.StatusCode, content);
        }

        _logger.LogWarning("Failed to generate image prompt for completion prompt: {0}", completionPrompt);
        return null;
    }

    private static string ProcessImagePrompt(string imagePrompt)
    {
        var trimmed = imagePrompt.Trim(new[] { ' ', '\n', '\r' });
        return trimmed;
    }

    private string ChooseRandomPrompt() => CompletionPrompts.AllPrompts[_random.Next(CompletionPrompts.AllPrompts.Count)];
}
