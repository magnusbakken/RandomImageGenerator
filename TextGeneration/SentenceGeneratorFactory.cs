using Markov;
using System.Collections.Concurrent;

namespace RandomImageGenerator.TextGeneration;

public class SentenceGeneratorFactory : ISentenceGeneratorFactory
{
    private readonly ConcurrentDictionary<string, ISentenceGenerator> _mapping = new();

    public ISentenceGenerator CreateGenerator(string corpus) => _mapping.GetOrAdd(corpus, corpus => new SentenceGenerator(CreateChain(corpus)));

    private static MarkovChain<string> CreateChain(string corpus)
    {
        var lines = File.ReadAllLines(corpus)
            .Select(line => line.Replace("”", "").Replace("“", ""))
            .ToArray();

        var chain = new MarkovChain<string>(1);
        foreach (var line in lines)
            chain.Add(line.Split(' '));

        return chain;
    }
}
