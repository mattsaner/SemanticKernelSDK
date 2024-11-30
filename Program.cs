#pragma warning disable SKEXP0050 // Using the 'Microsoft.SemanticKernel' namespace is not recommended. Use 'Microsoft.SemanticKernel.Plugins.Core' instead.

using Microsoft.SemanticKernel; // For OpenAI Service interaction
using Microsoft.SemanticKernel.Plugins.Core; // For OpenAI Service interaction


//Define OpenAI Service connection details
var deploymentName = "MastercamCopilot";
var endPoint = "https://az2005-training.openai.azure.com/";
var apiKey = Environment.GetEnvironmentVariable("OPEN_AI_SERVICE_KEY");
var modelId = "gpt-4o";

// Build Semantic Kernel
var builder = Kernel.CreateBuilder();
// Add the OpenAI Chat completion plugin to the kernel builder
builder.AddAzureOpenAIChatCompletion(deploymentName, endPoint, apiKey, modelId);
// Add the core plugins to the kernel builder for timer and logging
builder.Plugins.AddFromType<TimePlugin>();

var kernel = builder.Build();       

// Invoke the prompt  
var currentDay = await kernel.InvokeAsync("TimePlugin", "DayOfWeek");
Console.WriteLine(currentDay);
