namespace RandomImageGenerator.TextGeneration;

public interface ISentenceGeneratorFactory
{
    ISentenceGenerator CreateGenerator(TextGeneratorType textGeneratorType, SentenceGeneratorOptions options);
}