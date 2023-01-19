using Microsoft.Extensions.DependencyInjection;

namespace RandomImageGenerator.TextGeneration;

public static class TextGenerationServiceCollectionExtensions
{
    public static IServiceCollection AddTextGeneration(this IServiceCollection services)
    {
        services.AddSingleton<ISentenceGeneratorFactory, SentenceGeneratorFactory>();

        return services;
    }
}