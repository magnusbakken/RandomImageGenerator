namespace RandomImageGenerator.Generation;

public abstract record GeneratorResult
{
    public static AccessDeniedResult AccessDenied { get; } = new();

    public static ImageGenerationFailedResult ImageGenerationFailed { get; } = new();

    public static SuccessResult Success(byte[] image, string sentence) => new(image, sentence);

    private GeneratorResult() { }

    public record AccessDeniedResult : GeneratorResult { }

    public record ImageGenerationFailedResult : GeneratorResult { }

    public record SuccessResult : GeneratorResult
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
