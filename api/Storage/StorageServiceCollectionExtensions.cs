using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RandomImageGenerator.Storage;

public static class StorageCollectionExtensions
{
    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        services.AddOptions<AzureStorageOptions>()
            .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(nameof(AzureStorageOptions)).Bind(settings));
        services.AddTransient<IStorageHandler, AzureStorageHandler>();

        return services;
    }
}