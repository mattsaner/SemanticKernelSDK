#pragma warning disable SKEXP0050 // Using the 'Microsoft.SemanticKernel' namespace is not recommended. Use 'Microsoft.SemanticKernel.Plugins.Core' instead.

using Microsoft.SemanticKernel; // For OpenAI Service interaction
using Microsoft.SemanticKernel.Plugins.Core; // For OpenAI Service interaction
using Microsoft.SemanticKernel.ChatCompletion; // For OpenAI Service interaction . ChatHistory

//Define OpenAI Service connection details
var deploymentName = "gpt-35-turbo";
var endPoint = "https://az2005-training.openai.azure.com/";
var apiKey = Environment.GetEnvironmentVariable("OPEN_AI_SERVICE_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("API key is not set. Please set the OPEN_AI_SERVICE_KEY environment variable.");
    return;
}
var modelId = "gpt-35-turbo"; // Other models produce side effects so I stick to the one of the training

// Create a new kernel builder
var builder = Kernel.CreateBuilder();

// Add the OpenAI Chat completion plugin to the kernel builder
builder.AddAzureOpenAIChatCompletion(deploymentName, endPoint, apiKey, modelId);

// Build the kernel
var customKernel = builder.Build();   

// Import the custom MusicLibraryPlugin
customKernel.ImportPluginFromType<MusicLibraryPlugin>();

// Invoke the custom plugin AddToRecentlyPlayed function
/*var result = await customKernel.InvokeAsync(
    "MusicLibraryPlugin", 
    "AddToRecentlyPlayed", 
    new() {
        ["artist"] = "Tiara", 
        ["song"] = "Danse", 
        ["genre"] = "French pop, electropop, pop"
    }
);*/
string prompt = @"This is a list of music available to the user:
    {{MusicLibraryPlugin.GetMusicLibrary}} 

    This is a list of music the user has recently played:
    {{MusicLibraryPlugin.GetRecentPlays}}

    Based on their recently played music, suggest a song from
    the list to play next";

var result = await customKernel.InvokePromptAsync(prompt);

Console.WriteLine(result);
    
