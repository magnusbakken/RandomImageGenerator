using Microsoft.Extensions.DependencyInjection;
using RandomImageGenerator.Generation.ImageGeneration.DeepAI;
using RandomImageGenerator.Generation.ImageGeneration.OpenAI;
using RandomImageGenerator.Generation.TextGeneration.OpenAI;

namespace RandomImageGenerator.Generation.ImageGeneration;

public static class ImageGenerationServiceCollectionExtensions
{
    public static IServiceCollection AddImageGeneration(this IServiceCollection services)
    {
        services.AddDeepAIImageGenerator();
        services.AddOpenAIImageGenerator();
        services.AddTransient<IImageGeneratorFactory, ImageGeneratorFactory>();

        return services;
    }
}