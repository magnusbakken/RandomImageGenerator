namespace RandomImageGenerator.Generation.TextGeneration;

public interface ISentenceGeneratorFactory
{
    ISentenceGenerator CreateGenerator(TextGeneratorType textGeneratorType, SentenceGeneratorOptions options);
}