using Microsoft.SemanticKernel; // For OpenAI Service interaction


//Define OpenAI Service connection details
var deploymentName = "MastercamCopilot";
var endPoint = "https://az2005-training.openai.azure.com/";
var apiKey = Environment.GetEnvironmentVariable("OPEN_AI_SERVICE_KEY");
var modelId = "gpt-4o";
const string switchInputMode = "Switch input mode";

// Build Semantic Kernel
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(deploymentName, endPoint, apiKey, modelId);
var kernel = builder.Build();       

var result = await kernel.InvokePromptAsync("Give me a list of good movie to watch for a computer scientist?");

// See https://aka.ms/new-console-template for more information
Console.WriteLine(result);
