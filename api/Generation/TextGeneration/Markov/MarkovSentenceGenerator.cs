﻿using Markov;

namespace RandomImageGenerator.Generation.TextGeneration.Markov;

public class MarkovSentenceGenerator : ISentenceGenerator
{
    private readonly MarkovChain<string> _chain;

    public MarkovSentenceGenerator(string corpusPath) => _chain = CreateChain(corpusPath);

    public Task<string?> Generate(CancellationToken cancellationToken) => Task.FromResult<string?>(string.Join(" ", _chain.Chain()));

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
