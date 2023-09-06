namespace RandomImageGenerator.Generation.ImageGeneration.Picsart;

public record PicsartOptions
{
    public string ApiKey { get; init; } = "";

    public string NegativePrompt { get; init; } = "Not safe for work";

    public int Width { get; init; } = 1024;

    public int Height { get; init; } = 1024;
}
