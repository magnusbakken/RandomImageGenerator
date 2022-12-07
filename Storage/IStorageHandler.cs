namespace RandomImageGenerator.Storage;

public interface IStorageHandler
{
    Task<string> StoreImage(byte[] image, CancellationToken cancellationToken);
}