using RandomImageGenerator.Generation.ImageGeneration.DeepAI;
using RandomImageGenerator.Generation.ImageGeneration.OpenAI;
using RandomImageGenerator.Generation.ImageGeneration.Picsart;

namespace RandomImageGenerator.Generation.ImageGeneration;

public class ImageGeneratorFactory : IImageGeneratorFactory
{
    private readonly DeepAIGenerator _deepAIGenerator;
    private readonly OpenAIImageGenerator _openAIGenerator;
    private readonly DeepAIStableDiffusionGenerator _deepAIStableDiffusionGenerator;
    private readonly PicsartGenerator _picsartGenerator;

    public ImageGeneratorFactory(
        DeepAIGenerator deepAIGenerator,
        OpenAIImageGenerator openAIGenerator,
        DeepAIStableDiffusionGenerator stableDiffusionGenerator,
        PicsartGenerator picsartGenerator)
    {
        _deepAIGenerator = deepAIGenerator;
        _openAIGenerator = openAIGenerator;
        _deepAIStableDiffusionGenerator = stableDiffusionGenerator;
        _picsartGenerator = picsartGenerator;
    }

    public IImageGenerator Create(ImageGeneratorType generatorType)
    {
        return generatorType switch
        {
            ImageGeneratorType.DeepAI => _deepAIGenerator,
            ImageGeneratorType.OpenAI => _openAIGenerator,
            ImageGeneratorType.DeepAIStableDiffusion => _deepAIStableDiffusionGenerator,
            ImageGeneratorType.Picsart => _picsartGenerator,
            _ => throw new ArgumentException($"Unknown image generator type: {generatorType}", nameof(generatorType)),
        };
    }
}