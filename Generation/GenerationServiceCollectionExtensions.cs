using Microsoft.Extensions.DependencyInjection;

namespace RandomImageGenerator.Generation;

public static class GenerationServiceCollectionExtensions
{
    public static IServiceCollection AddGeneration(this IServiceCollection services)
    {
        services.AddTransient<IGenerator, Generator>();

        return services;
    }
}