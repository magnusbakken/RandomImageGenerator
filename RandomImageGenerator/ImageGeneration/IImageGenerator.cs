namespace RandomImageGenerator.ImageGeneration;

public interface IImageGenerator
{
    Task<byte[]> Generate(string sentence, CancellationToken cancellationToken);
}