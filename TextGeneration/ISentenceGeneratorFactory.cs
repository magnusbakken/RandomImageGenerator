namespace RandomImageGenerator.TextGeneration;

public interface ISentenceGeneratorFactory
{
    ISentenceGenerator CreateGenerator(string corpus);
}