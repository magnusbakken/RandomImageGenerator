namespace RandomImageGenerator.Shared.OpenAI;

public record OpenAIOptions
{
    public string ApiKey { get; init; } = "";

    public string CompletionModel { get; init; } = "";

    public int CompletionMaxTokens { get; init; }

    public double CompletionTemperature { get; init; }
}
