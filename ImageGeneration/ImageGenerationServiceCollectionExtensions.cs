using Microsoft.Extensions.DependencyInjection;
using RandomImageGenerator.ImageGeneration.DeepAI;
using RandomImageGenerator.ImageGeneration.OpenAI;
using RandomImageGenerator.TextGeneration.OpenAI;

namespace RandomImageGenerator.ImageGeneration;

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