namespace RandomImageGenerator.Generation.TextGeneration;

public record SentenceGeneratorOptions
{
    public string MarkovCorpusPath { get; init; } = "";
}