using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
#pragma warning disable SKEXP0050 
#pragma warning disable SKEXP0060

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

// Add the OpenAI Chat completion plugin to the kernel builder
var kernel = builder.Build();

// Note: ChatHistory isn't working correctly as of SemanticKernel v 1.4.0
StringBuilder chatHistory = new();

kernel.ImportPluginFromType<CurrencyConverter>();
kernel.ImportPluginFromType<ConversationSummaryPlugin>();
var prompts = kernel.ImportPluginFromPromptDirectory("Prompts");


string input;

do 
{
    Console.WriteLine("What would you like to do?");
    input = Console.ReadLine();

    OpenAIPromptExecutionSettings settings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    var intent = await kernel.InvokeAsync<string>(
        prompts["GetIntent"], 
        new() {{ "input",  input }}
    );

    switch (intent) {
        case "ConvertCurrency": 
            var currencyText = await kernel.InvokeAsync<string>(
                prompts["GetTargetCurrencies"], 
                new() {{ "input",  input }}
            );
            var currencyInfo = currencyText!.Split("|");
            var result = await kernel.InvokeAsync("CurrencyConverter", 
                "ConvertAmount", 
                new() {
                    {"targetCurrencyCode", currencyInfo[0]}, 
                    {"baseCurrencyCode", currencyInfo[1]},
                    {"amount", currencyInfo[2]}, 
                }
            );
            Console.WriteLine(result);
            break;
        case "SuggestDestinations":
            chatHistory.AppendLine("User:" + input);
            var recommendations = await kernel.InvokePromptAsync(input!);
            Console.WriteLine(recommendations);
    break;
        case "SuggestActivities":
            var chatSummary = await kernel.InvokeAsync(
                "ConversationSummaryPlugin", 
            "SummarizeConversation", 
            new() {{ "input", chatHistory.ToString() }});
            var activities = await kernel.InvokePromptAsync(
                input,
                new () {
                {"input", input},
                {"history", chatSummary},
                {"ToolCallBehavior", ToolCallBehavior.AutoInvokeKernelFunctions}
            });
            chatHistory.AppendLine("User:" + input);
            chatHistory.AppendLine("Assistant:" + activities.ToString());
            Console.WriteLine(activities);
            break;
        case "HelpfulPhrases":
        case "Translate":
            var autoInvokeResult = await kernel.InvokePromptAsync(input!, new(settings));
            Console.WriteLine(autoInvokeResult);
            break;
        default:
            Console.WriteLine("Sure, I can help with that.");
            var otherIntentResult = await kernel.InvokePromptAsync(input!, new(settings));
            Console.WriteLine(otherIntentResult);
            break;
    }

}
while (!string.IsNullOrWhiteSpace(input));

/*var result = await kernel.InvokeAsync(prompts["GetTargetCurrencies"],
    new() {
        {"input", "How many Australian Dollars is 140,000 Korean Won worth?"}
    }
);
Console.WriteLine(result);*/

