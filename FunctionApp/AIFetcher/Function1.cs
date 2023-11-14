using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Security;

namespace AIFetcher
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

           
            return new OkObjectResult("SslClientHelloInfo");
        }
    }
}


//// Note: The Azure OpenAI client library for .NET is in preview.
//// Install the .NET library via NuGet: dotnet add package Azure.AI.OpenAI --version 1.0.0-beta.5
//using Azure;
//using Azure.AI.OpenAI;

//OpenAIClient client = new OpenAIClient(
//  new Uri("https://oai-devopsaiprototyping.openai.azure.com/"),
//  new AzureKeyCredential(Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")));

//Response<ChatCompletions> responseWithoutStream = await client.GetChatCompletionsAsync(
//"gpt-35-turbo",
//new ChatCompletionsOptions()
//{
//    Messages =
//  {
//      new ChatMessage(ChatRole.System, @"You are an AI assistant that helps people find information."),

//  },
//    Temperature = (float)0.7,
//    MaxTokens = 800,


//    NucleusSamplingFactor = (float)0.95,
//    FrequencyPenalty = 0,
//    PresencePenalty = 0,
//});


//ChatCompletions response = responseWithoutStream.Value;
