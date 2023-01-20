# RandomImageGenerator

A simple Azure Functions app that generates a random English sentence, then generates a random image based on the random sentence using either DeepAI or OpenAI as its image generator.

There are two different functions: `GenerateImage` and `GenerateImageLink`. `GenerateImage` generates an images directly and sets the auto-generated sentence as the suggested file name when downloading. `GenerateImageLink` returns a link to the generated image in Azure Storage, where it will be available for two days.

The image generation uses either [OpenAI](https://openai.com/) or [DeepAI](https://deepai.org/). The random prompt can either be generated using a Markov chain text generation with a customizable corpus, or using OpenAI's text completion model with a selection of predefined prompts.
