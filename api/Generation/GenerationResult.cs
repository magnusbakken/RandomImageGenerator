namespace RandomImageGenerator.Generation;

public abstract record GenerationResult<T>
{
    public static AccessDeniedResult AccessDenied { get; } = new();

    public static SentenceGenerationFailedResult SentenceGenerationFailed { get; } = new();

    public static ImageGenerationFailedResult ImageGenerationFailed { get; } = new();

    public static SuccessResult Success(T data) => new(data);

    private GenerationResult() { }

    public record AccessDeniedResult : GenerationResult<T> { }

    public record SentenceGenerationFailedResult : GenerationResult<T> { }

    public record ImageGenerationFailedResult : GenerationResult<T> { }

    public record SuccessResult : GenerationResult<T>
    {
        public SuccessResult(T data) => Data = data;

        public T Data { get; }
    }
}
