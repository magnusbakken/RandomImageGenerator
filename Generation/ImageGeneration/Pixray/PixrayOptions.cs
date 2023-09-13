namespace RandomImageGenerator.Generation.ImageGeneration.Pixray;

public record PixrayOptions
{
    public string ApiKey { get; init; } = "";

    public string ApiVersion { get; init; } = "";

    public string Aspect { get; init; } = PixrayAspect.Square;

    public string Drawer { get; init; } = "vqgan";

    public string Quality { get; init; } = PixrayQuality.Normal;

    public double Scale { get; init; } = 1;

    public int PollDelayMs { get; init; } = 2000;

    public int MaxPollCount { get; init; } = 30;
}