using Microsoft.Extensions.DependencyInjection;
using RandomImageGenerator.Shared.OpenAI;

namespace RandomImageGenerator.Shared;

public static class SharedServiceCollectionExtensions
{
    public static IServiceCollection AddShared(this IServiceCollection services)
    {
        services.AddOpenAIShared();

        return services;
    }
}