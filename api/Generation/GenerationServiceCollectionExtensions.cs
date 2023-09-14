using Microsoft.Extensions.DependencyInjection;
using RandomImageGenerator.Generation.ImageGeneration;
using RandomImageGenerator.Generation.TextGeneration;

namespace RandomImageGenerator.Generation;

public static class GenerationServiceCollectionExtensions
{
    public static IServiceCollection AddGeneration(this IServiceCollection services)
    {
        services.AddTransient<IGenerator, Generator>();
        services.AddImageGeneration();
        services.AddTextGeneration();

        return services;
    }
}