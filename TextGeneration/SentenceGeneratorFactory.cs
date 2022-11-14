using Markov;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace RandomImageGenerator.TextGeneration;

public class SentenceGeneratorFactory : ISentenceGeneratorFactory
{
    private readonly ConcurrentDictionary<string, ISentenceGenerator> _mapping = new();

    public ISentenceGenerator CreateGenerator(string corpus) => _mapping.GetOrAdd(corpus, corpus => new SentenceGenerator(CreateChain(corpus)));

    private static MarkovChain<string> CreateChain(string corpus)
    {
        var regex = new Regex(@"^\d+\s+(.*)");
        var lines = File.ReadAllLines(corpus)
            .SelectMany(line => regex.Match(line) is Match match && match.Success ? new[] { match.Groups[1].Value } : Enumerable.Empty<string>())
            .Select(line => line.Replace("”", "").Replace("“", ""))
            .ToArray();

        var chain = new MarkovChain<string>(1);
        foreach (var line in lines)
            chain.Add(line.Split(' '));

        return chain;
    }
}
