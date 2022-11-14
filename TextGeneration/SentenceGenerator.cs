using Markov;

namespace RandomImageGenerator.TextGeneration;

public class SentenceGenerator : ISentenceGenerator
{
    private readonly MarkovChain<string> _chain;

    public SentenceGenerator(MarkovChain<string> chain) => _chain = chain;

    public string Generate() => string.Join(" ", _chain.Chain());
}
