namespace RandomImageGenerator.ImageGeneration;

public class DeepAIGenerator : IImageGenerator
{
    private const string Url = "https://api.deepai.org/api/text2img";
    private const string ApiKey = "quickstart-QUdJIGlzIGNvbWluZy4uLi4K";

    private readonly HttpClient _httpClient;

    public DeepAIGenerator(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("api-key", ApiKey);
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
        
        return Array.Empty<byte>();
    }
}
