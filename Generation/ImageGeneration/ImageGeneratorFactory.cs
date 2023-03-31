using RandomImageGenerator.Generation.ImageGeneration.DeepAI;
using RandomImageGenerator.Generation.ImageGeneration.OpenAI;

namespace RandomImageGenerator.Generation.ImageGeneration;

public class ImageGeneratorFactory : IImageGeneratorFactory
{
    private readonly DeepAIGenerator _deepAIGenerator;
    private readonly OpenAIImageGenerator _openAIGenerator;
    private readonly DeepAIStableDiffusionGenerator _deepAIStableDiffusionGenerator;

    public ImageGeneratorFactory(
        DeepAIGenerator deepAIGenerator,
        OpenAIImageGenerator openAIGenerator,
        DeepAIStableDiffusionGenerator stableDiffusionGenerator)
    {
        _deepAIGenerator = deepAIGenerator;
        _openAIGenerator = openAIGenerator;
        _deepAIStableDiffusionGenerator = stableDiffusionGenerator;
    }

    public IImageGenerator Create(ImageGeneratorType generatorType)
    {
        return generatorType switch
        {
            ImageGeneratorType.DeepAI => _deepAIGenerator,
            ImageGeneratorType.OpenAI => _openAIGenerator,
            ImageGeneratorType.DeepAIStableDiffusion => _deepAIStableDiffusionGenerator,
            _ => throw new ArgumentException($"Unknown image generator type: {generatorType}", nameof(generatorType)),
        };
    }
}