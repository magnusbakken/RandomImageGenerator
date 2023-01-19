using RandomImageGenerator.ImageGeneration.DeepAI;
using RandomImageGenerator.ImageGeneration.OpenAI;

namespace RandomImageGenerator.ImageGeneration;

public class ImageGeneratorFactory : IImageGeneratorFactory
{
    private readonly DeepAIGenerator _deepAIGenerator;
    private readonly OpenAIGenerator _openAIGenerator;

    public ImageGeneratorFactory(DeepAIGenerator deepAIGenerator, OpenAIGenerator openAIGenerator)
    {
        _deepAIGenerator = deepAIGenerator;
        _openAIGenerator = openAIGenerator;
    }

    public IImageGenerator Create(ImageGeneratorType generatorType)
    {
        return generatorType switch
        {
            ImageGeneratorType.DeepAI => _deepAIGenerator,
            ImageGeneratorType.OpenAI => _openAIGenerator,
            _ => throw new ArgumentException($"Unknown image generator type: {generatorType}", nameof(generatorType)),
        };
    }
}