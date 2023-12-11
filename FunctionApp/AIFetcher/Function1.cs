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

            OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));




            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
    {
        new ChatMessage(ChatRole.System, "You are an assistant that provides 2 forms of functionality: a regular chat functionality that is centered on describing and understanding a software product, and a JSON-crafter that output Azure DevOps work items in a simple JSON format for review."),        
        //new ChatMessage(ChatRole.System, "Each work item should be a array consisting of objects with fields 'op',  'title', and 'description'. An example of an valid object is {\"id\": 1, \"title\": \"Sample Task\", \"description\": \"This is a sample description.\"} "),
        new ChatMessage(ChatRole.System, "The chat message should be in the first part of the message, and the workItem-json output in the last part."),
        new ChatMessage(ChatRole.System, "Format each work item as an array containing at least one object. Each object must have the keys 'from', 'op', 'path', and 'value'. "),
        new ChatMessage(ChatRole.System, "For the Title object Set 'from' to null, 'op' to 'add', and 'path' to '/fields/System.Title'. The 'value' should contain the task's title. For example: [{\"from\": null, \"op\": \"add\", \"path\": \"/fields/System.Title\", \"value\": \"Sample task\"}]"),
        new ChatMessage(ChatRole.System, "For the Description object Set 'from' to null, 'op' to 'add', and 'path' to '/fields/System.Description'. The 'value' should contain the task's description."),
        new ChatMessage(ChatRole.System, "You should provide each workitem with an Title and corresponding description"),       
        new ChatMessage(ChatRole.System, "the arrays containing the individual workitems should be collected in an array using square brackets [] and given the name workitems. An example of an valid array output is \"workItems\": [[{object1}],[{object2}], etc..]"),
        new ChatMessage(ChatRole.System, "the data should be enclosed in {} as in JSON format. and example is {\"workItems\":[[{object1}],[{object2}]}"),
        //new ChatMessage(ChatRole.User, $"{input}"),
    },
                MaxTokens = 1000
            };

            //Adds the user input to the message history
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, $"{input}"));

            Response<ChatCompletions> response = client.GetChatCompletions(
                deploymentOrModelName: "gpt-35-turbo",
                chatCompletionsOptions);

            Console.WriteLine("Response content before json extract");
            Console.WriteLine(response.Value.Choices[0].Message.Content);

            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.Assistant, response.Value.Choices[0].Message.Content));

            Console.WriteLine("message history:");
            foreach (var item in chatCompletionsOptions.Messages)
            {
                Console.WriteLine(item.Content);    
            }   
            
            //extract the JSON from the response and return it
            try
            {
                var jsonResponseObject = JSONExtractor.ExtractJson(response.Value.Choices[0].Message.Content);
                Console.WriteLine("Response content after json extract");
                return new OkObjectResult(jsonResponseObject);
            }
            catch (InvalidOperationException ex)
            {
                return new OkObjectResult("json validation failed");
            }
            
        }
    }
}
