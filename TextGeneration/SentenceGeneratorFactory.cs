using RandomImageGenerator.TextGeneration.Markov;
using System.Collections.Concurrent;

namespace RandomImageGenerator.TextGeneration;

public class SentenceGeneratorFactory : ISentenceGeneratorFactory
{
    private readonly ConcurrentDictionary<string, MarkovSentenceGenerator> _corpusMarkovMapping = new();

    public ISentenceGenerator CreateGenerator(TextGeneratorType textGeneratorType, SentenceGeneratorOptions options)
    {
        return textGeneratorType switch
        {
            TextGeneratorType.Markov => CreateMarkovGenerator(options),
            _ => throw new ArgumentException($"Unknown text generator type: {textGeneratorType}", nameof(textGeneratorType)),
        };
    }

    private MarkovSentenceGenerator CreateMarkovGenerator(SentenceGeneratorOptions options)
    {
        return _corpusMarkovMapping.GetOrAdd(options.MarkovCorpusPath, corpusPath => new MarkovSentenceGenerator(corpusPath));
    }
}
