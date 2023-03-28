using RandomImageGenerator.Generation.TextGeneration.Markov;
using RandomImageGenerator.Generation.TextGeneration.OpenAI;
using System.Collections.Concurrent;

namespace RandomImageGenerator.Generation.TextGeneration;

public class SentenceGeneratorFactory : ISentenceGeneratorFactory
{
    private readonly ConcurrentDictionary<string, MarkovSentenceGenerator> _corpusMarkovMapping = new();
    private readonly OpenAISentenceGenerator _openAIGenerator;

    public SentenceGeneratorFactory(OpenAISentenceGenerator openAIGenerator)
    {
        _openAIGenerator = openAIGenerator;
    }

    public ISentenceGenerator CreateGenerator(TextGeneratorType textGeneratorType, SentenceGeneratorOptions options)
    {
        return textGeneratorType switch
        {
            TextGeneratorType.Markov => CreateMarkovGenerator(options),
            TextGeneratorType.OpenAI => CreateOpenAIGenerator(),
            _ => throw new ArgumentException($"Unknown text generator type: {textGeneratorType}", nameof(textGeneratorType)),
        };
    }

    private MarkovSentenceGenerator CreateMarkovGenerator(SentenceGeneratorOptions options)
    {
        return _corpusMarkovMapping.GetOrAdd(options.MarkovCorpusPath, corpusPath => new MarkovSentenceGenerator(corpusPath));
    }

    private OpenAISentenceGenerator CreateOpenAIGenerator() => _openAIGenerator;
}
