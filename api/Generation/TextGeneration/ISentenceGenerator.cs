namespace RandomImageGenerator.Generation.TextGeneration;

public interface ISentenceGenerator
{
    Task<string?> Generate(CancellationToken cancellationToken);
}