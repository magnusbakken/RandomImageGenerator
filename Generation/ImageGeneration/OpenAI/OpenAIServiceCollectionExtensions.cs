using Microsoft.Extensions.DependencyInjection;

namespace RandomImageGenerator.Generation.ImageGeneration.OpenAI;

public static class OpenAIServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAIImageGenerator(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(OpenAIImageGenerator));
        services.AddHttpClient(nameof(OpenAIImageDownloader));

        services.AddTransient<OpenAIImageGenerator>();
        services.AddTransient<OpenAIImageDownloader>();

        return services;
    }
}