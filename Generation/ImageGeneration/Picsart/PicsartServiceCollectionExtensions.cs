using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RandomImageGenerator.Generation.ImageGeneration.Picsart;

public static class PicsartServiceCollectionExtensions
{
    public static IServiceCollection AddPicsartImageGenerator(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(PicsartGenerator));

        services.AddOptions<PicsartOptions>()
            .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(nameof(PicsartOptions)).Bind(settings));

        services.AddTransient<PicsartGenerator>();

        return services;
    }
}