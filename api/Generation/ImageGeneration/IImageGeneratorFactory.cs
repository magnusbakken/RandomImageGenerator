namespace RandomImageGenerator.Generation.ImageGeneration;

public interface IImageGeneratorFactory
{
    IImageGenerator Create(ImageGeneratorType generatorType);
}
