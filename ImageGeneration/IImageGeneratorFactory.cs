namespace RandomImageGenerator.ImageGeneration;

public interface IImageGeneratorFactory
{
    IImageGenerator Create(ImageGeneratorType generatorType);
}
