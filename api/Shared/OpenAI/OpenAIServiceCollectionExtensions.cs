using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RandomImageGenerator.Shared.OpenAI;

public static class OpenAISharedServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAIShared(this IServiceCollection services)
    {
        services.AddOptions<OpenAIOptions>()
            .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(nameof(OpenAIOptions)).Bind(settings));

        return services;
    }
}