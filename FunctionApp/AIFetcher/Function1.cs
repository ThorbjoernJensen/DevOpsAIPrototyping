using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure;
using Azure.AI.OpenAI;
using System.Text;

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

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string input = data?.input;

            //// Note: The Azure OpenAI client library for .NET is in preview.
            //// Install the .NET library via NuGet: dotnet add package Azure.AI.OpenAI --version 1.0.0-beta.5


            string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            string key = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");

            // Enter the deployment name you chose when you deployed the model.
            string engine = "gpt-35-turbo";

            OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
    {
        new ChatMessage(ChatRole.System, "You are a helpful assistant."),
        new ChatMessage(ChatRole.User, $"{input}"),

    },
                MaxTokens = 100
            };

            Response<ChatCompletions> response = client.GetChatCompletions(
                deploymentOrModelName: "gpt-35-turbo",
                chatCompletionsOptions);

            Console.WriteLine(response.Value.Choices[0].Message.Content);

            return new OkObjectResult(response.Value.Choices[0].Message.Content);
        }
    }
}
