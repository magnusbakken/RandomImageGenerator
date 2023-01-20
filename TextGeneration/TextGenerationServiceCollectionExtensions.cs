using Microsoft.Extensions.DependencyInjection;
using RandomImageGenerator.TextGeneration.OpenAI;

namespace RandomImageGenerator.TextGeneration;

public static class TextGenerationServiceCollectionExtensions
{
    public static IServiceCollection AddTextGeneration(this IServiceCollection services)
    {
        services.AddOpenAISentenceGenerator();
        services.AddSingleton<ISentenceGeneratorFactory, SentenceGeneratorFactory>();

        return services;
    }
}