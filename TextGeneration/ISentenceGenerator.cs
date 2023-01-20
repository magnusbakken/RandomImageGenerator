namespace RandomImageGenerator.TextGeneration;

public interface ISentenceGenerator
{
    Task<string?> Generate(CancellationToken cancellationToken);
}