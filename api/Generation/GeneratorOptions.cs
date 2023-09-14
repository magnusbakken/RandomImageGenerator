using RandomImageGenerator.Corpora;
using RandomImageGenerator.Generation.ImageGeneration;
using RandomImageGenerator.Generation.TextGeneration;

namespace RandomImageGenerator.Generation;

public record GeneratorOptions(TextGeneratorType TextGeneratorType, ImageGeneratorType ImageGeneratorType, Corpus Corpus);