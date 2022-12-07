using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RandomImageGenerator.Storage;

public class AzureStorageHandler : IStorageHandler
{
    private readonly AzureStorageOptions _options;
    private readonly ILogger<AzureStorageHandler> _logger;

    public AzureStorageHandler(IOptions<AzureStorageOptions> options, ILogger<AzureStorageHandler> log)
    {
        _options = options.Value;
        _logger = log;
    }

    public async Task<string> StoreImage(byte[] image, CancellationToken cancellationToken)
    {
        var name = $"{Guid.NewGuid()}.jpg";
        var client = new BlobServiceClient(_options.ConnectionString);
        var containerClient = client.GetBlobContainerClient(_options.BlobContainerName);
        await containerClient.UploadBlobAsync(name, new BinaryData(image), cancellationToken);
        var builder = new UriBuilder
        {
            Scheme = "https",
            Host = $"{_options.StorageName}.blob.core.windows.net",
            Path = $"{_options.BlobContainerName}/{name}"
        };

        var uri = builder.ToString();
        _logger.LogInformation("Uploaded image to blog storage: {Url}", uri);
        return uri;
    }
}
