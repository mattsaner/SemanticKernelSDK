#pragma warning disable SKEXP0050 // Using the 'Microsoft.SemanticKernel' namespace is not recommended. Use 'Microsoft.SemanticKernel.Plugins.Core' instead.

using Microsoft.SemanticKernel; // For OpenAI Service interaction
using Microsoft.SemanticKernel.Plugins.Core; // For OpenAI Service interaction


//Define OpenAI Service connection details
var deploymentName = "gpt-35-turbo";
var endPoint = "https://az2005-training.openai.azure.com/";
var apiKey = Environment.GetEnvironmentVariable("OPEN_AI_SERVICE_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("API key is not set. Please set the OPEN_AI_SERVICE_KEY environment variable.");
    return;
}
var modelId = "gpt-35-turbo";

// Build Semantic Kernel
var builder = Kernel.CreateBuilder();
// Add the OpenAI Chat completion plugin to the kernel builder
builder.AddAzureOpenAIChatCompletion(deploymentName, endPoint, apiKey, modelId);
// Add the core plugins to the kernel builder for timer and logging
//builder.Plugins.AddFromType<TimePlugin>();
// Add the ConversationSummaryPlugin to the kernel builder
builder.Plugins.AddFromType<ConversationSummaryPlugin>();

var kernel = builder.Build();    

// Define the input prompt
string input = @"I'm planning an anniversary trip with my spouse. We like hiking, mountains, 
    and beaches. Our travel budget is $15000";

// Assign a persona to the prompt
string prompt = @$"
    The following is a conversation with an AI travel assistant. 
    The assistant is helpful, creative, and very friendly.

    <message role=""user"">Can you give me some travel destination suggestions?</message>

    <message role=""assistant"">Of course! Do you have a budget or any specific 
    activities in mind?</message>

    <message role=""user"">${input}</message>";

var result = await kernel.InvokePromptAsync(prompt);
Console.WriteLine(result);