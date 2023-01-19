using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RandomImageGenerator.ImageGeneration.DeepAI;

public static class DeepAIServiceCollectionExtensions
{
    public static IServiceCollection AddDeepAI(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(DeepAIGenerator));

        services.AddOptions<DeepAIOptions>()
            .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(nameof(DeepAIOptions)).Bind(settings));

        services.AddTransient<DeepAIGenerator>();

        return services;
    }
}