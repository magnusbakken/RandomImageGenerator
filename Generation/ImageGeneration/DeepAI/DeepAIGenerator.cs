using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RandomImageGenerator.Generation.ImageGeneration.DeepAI;

public class DeepAIGenerator : BaseDeepAIGenerator
{
    public DeepAIGenerator(IHttpClientFactory httpClientFactory, IOptions<DeepAIOptions> options, ILogger<DeepAIGenerator> log)
        : base(httpClientFactory, options, log)
    {
    }

    public override string Url => "https://api.deepai.org/api/text2img";

    public override string GeneratorName => "DeepAI";
}
