using Microsoft.Extensions.DependencyInjection;
using RandomImageGenerator.Generation.TextGeneration.OpenAI;

namespace RandomImageGenerator.Generation.TextGeneration;

public static class TextGenerationServiceCollectionExtensions
{
    public static IServiceCollection AddTextGeneration(this IServiceCollection services)
    {
        services.AddOpenAISentenceGenerator();
        services.AddSingleton<ISentenceGeneratorFactory, SentenceGeneratorFactory>();

        return services;
    }
}