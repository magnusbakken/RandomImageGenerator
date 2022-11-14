using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace RandomImageGenerator.ImageGeneration;

public class DeepAIGenerator : IImageGenerator
{
    private const string Url = "https://api.deepai.org/api/text2img";
    private readonly DeepAIOptions _options;
    private readonly HttpClient _httpClient;

    public DeepAIGenerator(IHttpClientFactory httpClientFactory, IOptions<DeepAIOptions> options)
    {
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient(nameof(DeepAIGenerator));
        _httpClient.DefaultRequestHeaders.Add("api-key", _options.ApiKey);
    }

    public async Task<byte[]> Generate(string sentence, CancellationToken cancellationToken)
    {
        var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("text", sentence) });
        var response = await _httpClient.PostAsync(Url, formContent, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<DeepAIResponse>(cancellationToken: cancellationToken);
            if (content != null)
                return await _httpClient.GetByteArrayAsync(content.OutputUrl, cancellationToken);
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine(content);
        }

        return Array.Empty<byte>();
    }
}
