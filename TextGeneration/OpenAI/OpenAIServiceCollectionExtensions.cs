using Microsoft.Extensions.DependencyInjection;

namespace RandomImageGenerator.TextGeneration.OpenAI;

public static class OpenAIServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAISentenceGenerator(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(OpenAISentenceGenerator));

        services.AddTransient<OpenAISentenceGenerator>();

        return services;
    }
}