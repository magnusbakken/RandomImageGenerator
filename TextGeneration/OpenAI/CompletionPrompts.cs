namespace RandomImageGenerator.TextGeneration.OpenAI;

public static class CompletionPrompts
{
    public const string CreateAPrompt = "Write a prompt for an auto-generated image";
    public const string DescribeAScene = "Describe a visually interesting scene";
    public const string SaySomethingRidiculous = "Say something ridiculous";
    public const string WriteARandomSentence = "Write a random sentence";

    public static readonly IReadOnlyList<string> AllPrompts = new[] { CreateAPrompt, DescribeAScene, SaySomethingRidiculous, WriteARandomSentence };
}