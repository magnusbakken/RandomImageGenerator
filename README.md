# RandomImageGenerator

A simple Azure Functions app that generates a random English sentence using a Markov chain generator, then generates a random image based on the random sentence using either DeepAI or OpenAI as its image generator.

There are two different functions: `GenerateImage` and `GenerateImageLink`. `GenerateImage` generates an images directly and sets the auto-generated sentence as the suggested file name when downloading. `GenerateImageLink` returns a link to the generated image in Azure Storage, where it will be available for two days.
