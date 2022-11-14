using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RandomImageGenerator.Generation;
using RandomImageGenerator.ImageGeneration;
using RandomImageGenerator.SafeList;
using RandomImageGenerator.TextGeneration;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddLogging();
        services.AddHttpClient();
        services.AddOptions();

        services.AddSingleton<ISentenceGeneratorFactory, SentenceGeneratorFactory>();
        services.AddHttpClient(nameof(DeepAIGenerator));

        services.AddOptions<SafeListOptions>()
            .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(nameof(SafeListOptions)).Bind(settings));

        services.AddOptions<DeepAIOptions>()
            .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(nameof(DeepAIOptions)).Bind(settings));

        services.AddTransient<DeepAIGenerator>();
        services.AddTransient<IGenerator, Generator>();
    })
    .Build();

host.Run();
