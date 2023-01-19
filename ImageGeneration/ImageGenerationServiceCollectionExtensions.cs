using Microsoft.Extensions.DependencyInjection;
using RandomImageGenerator.ImageGeneration.DeepAI;
using RandomImageGenerator.ImageGeneration.OpenAI;

namespace RandomImageGenerator.ImageGeneration;

public static class ImageGenerationServiceCollectionExtensions
{
    public static IServiceCollection AddImageGeneration(this IServiceCollection services)
    {
        services.AddDeepAI();
        services.AddOpenAI();
        services.AddTransient<IImageGeneratorFactory, ImageGeneratorFactory>();

        return services;
    }
}