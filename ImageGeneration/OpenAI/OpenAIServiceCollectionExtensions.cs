using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RandomImageGenerator.ImageGeneration.OpenAI;

public static class OpenAIServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAI(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(OpenAIGenerator));
        services.AddHttpClient(nameof(OpenAIDownloader));
        
        services.AddOptions<OpenAIOptions>()
            .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(nameof(OpenAIOptions)).Bind(settings));

        services.AddTransient<OpenAIGenerator>();
        services.AddTransient<OpenAIDownloader>();
        
        return services;
    }
}