namespace RandomImageGenerator.Generation.ImageGeneration;

public interface IImageGenerator
{
    Task<byte[]> Generate(string sentence, CancellationToken cancellationToken);
}