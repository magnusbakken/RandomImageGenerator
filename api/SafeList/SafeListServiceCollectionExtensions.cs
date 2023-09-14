using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RandomImageGenerator.SafeList;

public static class SafeListServiceCollectionExtensions
{
    public static IServiceCollection AddSafeList(this IServiceCollection services)
    {
        services.AddOptions<SafeListOptions>()
            .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(nameof(SafeListOptions)).Bind(settings));

        return services;
    }
}