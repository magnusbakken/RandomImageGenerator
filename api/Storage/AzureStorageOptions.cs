namespace RandomImageGenerator.Storage;

public record AzureStorageOptions
{
    public string ConnectionString { get; init; } = "";

    public string StorageName { get; init; } = "";

    public string BlobContainerName { get; init; } = "";
}
