using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RandomImageGenerator.Generation.ImageGeneration.Pixray;

public static class PixrayServiceCollectionExtensions
{
    public static IServiceCollection AddPixrayImageGenerator(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(PixrayImageGenerator));
        services.AddHttpClient(nameof(PixrayImageDownloader));

        services.AddOptions<PixrayOptions>()
            .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(nameof(PixrayOptions)).Bind(settings));

        services.AddTransient<PixrayImageGenerator>();
        services.AddTransient<PixrayImageDownloader>();

        return services;
    }
}