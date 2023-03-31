using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RandomImageGenerator.Generation.ImageGeneration.DeepAI;

public class DeepAIStableDiffusionGenerator : BaseDeepAIGenerator
{
    public DeepAIStableDiffusionGenerator(IHttpClientFactory httpClientFactory, IOptions<DeepAIOptions> options, ILogger<DeepAIStableDiffusionGenerator> log)
        : base(httpClientFactory, options, log)
    {
    }

    public override string Url => "https://api.deepai.org/api/stable-diffusion";

    public override string GeneratorName => "DeepAI Stable Diffusion";
}
