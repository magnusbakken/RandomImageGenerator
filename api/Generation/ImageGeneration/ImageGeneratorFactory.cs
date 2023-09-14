using RandomImageGenerator.Generation.ImageGeneration.DeepAI;
using RandomImageGenerator.Generation.ImageGeneration.OpenAI;
using RandomImageGenerator.Generation.ImageGeneration.Pixray;

namespace RandomImageGenerator.Generation.ImageGeneration;

public class ImageGeneratorFactory : IImageGeneratorFactory
{
    private readonly DeepAIGenerator _deepAIGenerator;
    private readonly OpenAIImageGenerator _openAIGenerator;
    private readonly DeepAIStableDiffusionGenerator _deepAIStableDiffusionGenerator;
    private readonly PixrayImageGenerator _pixrayGenerator;

    public ImageGeneratorFactory(
        DeepAIGenerator deepAIGenerator,
        OpenAIImageGenerator openAIGenerator,
        DeepAIStableDiffusionGenerator stableDiffusionGenerator,
        PixrayImageGenerator pixrayGenerator)
    {
        _deepAIGenerator = deepAIGenerator;
        _openAIGenerator = openAIGenerator;
        _deepAIStableDiffusionGenerator = stableDiffusionGenerator;
        _pixrayGenerator = pixrayGenerator;
    }

    public IImageGenerator Create(ImageGeneratorType generatorType)
    {
        return generatorType switch
        {
            ImageGeneratorType.DeepAI => _deepAIGenerator,
            ImageGeneratorType.OpenAI => _openAIGenerator,
            ImageGeneratorType.DeepAIStableDiffusion => _deepAIStableDiffusionGenerator,
            ImageGeneratorType.Pixray => _pixrayGenerator,
            _ => throw new ArgumentException($"Unknown image generator type: {generatorType}", nameof(generatorType)),
        };
    }
}