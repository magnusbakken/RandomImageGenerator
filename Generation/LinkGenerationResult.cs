namespace RandomImageGenerator.Generation;

public abstract record LinkGenerationResult
{
    public static AccessDeniedResult AccessDenied { get; } = new();

    public static ImageGenerationFailedResult ImageGenerationFailed { get; } = new();

    public static SuccessResult Success(string link, string sentence) => new(link, sentence);

    private LinkGenerationResult() { }

    public record AccessDeniedResult : LinkGenerationResult { }

    public record ImageGenerationFailedResult : LinkGenerationResult { }

    public record SuccessResult : LinkGenerationResult
    {
        public SuccessResult(string link, string sentence)
        {
            Link = link;
            Sentence = sentence;
        }

        public string Link { get; }

        public string Sentence { get; }
    }
}
