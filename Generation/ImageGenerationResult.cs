namespace RandomImageGenerator.Generation;

public abstract record ImageGenerationResult
{
    public static AccessDeniedResult AccessDenied { get; } = new();

    public static ImageGenerationFailedResult ImageGenerationFailed { get; } = new();

    public static SuccessResult Success(byte[] image, string sentence) => new(image, sentence);

    private ImageGenerationResult() { }

    public record AccessDeniedResult : ImageGenerationResult { }

    public record ImageGenerationFailedResult : ImageGenerationResult { }

    public record SuccessResult : ImageGenerationResult
    {
        public SuccessResult(byte[] image, string sentence)
        {
            Image = image;
            Sentence = sentence;
        }

        public byte[] Image { get; }

        public string Sentence { get; }
    }
}
