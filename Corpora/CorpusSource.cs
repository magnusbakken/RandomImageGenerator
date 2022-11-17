using System.Collections.Generic;

namespace RandomImageGenerator.Corpora;

public static class CorpusSource
{
    private static readonly Dictionary<Corpus, string> _corpusPathMapping = new()
    {
        [Corpus.EngNews202010K] = "Corpora/eng_news_2020_10K/eng_news_2020_10K-sentences.txt",
        [Corpus.EngUkWebPublic201810K] = "Corpora/eng-uk_web-public_2018_10K/eng-uk_web-public_2018_10K-sentences.txt",
    };

    public static string GetCorpusPath(Corpus corpus) => _corpusPathMapping[corpus];
}
