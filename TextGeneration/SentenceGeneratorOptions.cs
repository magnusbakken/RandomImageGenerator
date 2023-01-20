namespace RandomImageGenerator.TextGeneration;

public record SentenceGeneratorOptions
{
    public string MarkovCorpusPath { get; init; } = "";
}