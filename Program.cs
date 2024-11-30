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

var prompts = kernel.ImportPluginFromPromptDirectory("Prompts/TravelPlugins");

//Import the plugins you created. You also use a ChatHistory object to store the user's conversation. Finally, pass some information to the SuggestDestinations prompt and record the results. 
//Next, let's ask the user where they want to go so we can recommend some activities to them.
ChatHistory history = [];
string input = @"I'm planning an anniversary trip with my spouse. We like hiking, 
    mountains, and beaches. Our travel budget is $15000";

var result = await kernel.InvokeAsync<string>(prompts["SuggestDestinations"],
    new() {{ "input", input }});

Console.WriteLine(result);
history.AddUserMessage(input);
history.AddAssistantMessage(result);

//In this code, you get some input from the user to find out where they want to go. 
//Then call the SuggestActivities prompt with the destination and the conversation history.
Console.WriteLine("Where would you like to go?");
input = Console.ReadLine();

result = await kernel.InvokeAsync<string>(prompts["SuggestActivities"],
    new() {
        { "history", history },
        { "destination", input },
    }
);
Console.WriteLine(result);
