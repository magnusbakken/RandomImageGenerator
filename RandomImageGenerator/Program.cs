using RandomImageGenerator.Corpora;
using RandomImageGenerator.ImageGeneration;
using RandomImageGenerator.SafeList;
using RandomImageGenerator.TextGeneration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ISentenceGeneratorFactory, SentenceGeneratorFactory>();
builder.Services.AddHttpClient<DeepAIGenerator>();
builder.Services.Configure<SafeListOptions>(builder.Configuration.GetSection(nameof(SafeListOptions)));
builder.Services.AddTransient<SafeListMiddleware>();
var app = builder.Build();

app.UseMiddleware<SafeListMiddleware>();
app.UseHttpsRedirection();

app.MapGet("/sentence", (ISentenceGeneratorFactory factory) =>
{
    return factory.CreateGenerator(CorpusSource.GetCorpusPath(Corpus.EngNews202010K)).Generate();
});

app.MapGet("/image", async (ISentenceGeneratorFactory sentenceGenFactory, DeepAIGenerator imageGenerator, CancellationToken cancellationToken) =>
{
    var sentence = sentenceGenFactory.CreateGenerator(CorpusSource.GetCorpusPath(Corpus.EngNews202010K)).Generate();
    var image = await imageGenerator.Generate(sentence, cancellationToken);
    if (image.Length == 0)
        return Results.BadRequest("Unable to generate image");

    return Results.Stream(new MemoryStream(image), contentType: "image/jpeg", fileDownloadName: $"{sentence}.jpg");
});

app.Run();