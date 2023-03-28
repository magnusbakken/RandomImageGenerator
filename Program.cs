using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RandomImageGenerator.Generation;
using RandomImageGenerator.SafeList;
using RandomImageGenerator.Shared;
using RandomImageGenerator.Storage;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddLogging();
        services.AddHttpClient();
        services.AddOptions();

        services.AddShared();
        services.AddGeneration();
        services.AddSafeList();
        services.AddStorage();
    })
    .Build();

host.Run();
