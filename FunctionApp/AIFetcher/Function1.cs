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
            //        var chatCompletionsOptions = new ChatCompletionsOptions()
            //        {
            //            Messages =
            //{
            //    new ChatMessage(ChatRole.System, "You are an assistant that only answers with azure devops workitems in json format."),
            //    new ChatMessage(ChatRole.System, "You do not reply with anything but json, as the json is to be parsed"),
            //    new ChatMessage(ChatRole.System, "You provide arrays of workitem objects following this schema {\r\n    \"type\": \"array\",\r\n    \"items\": {\r\n        \"type\": \"object\",\r\n        \"properties\": {\r\n            \"from\": {},\r\n            \"op\": {\r\n                \"type\": \"string\"\r\n            },\r\n            \"path\": {\r\n                \"type\": \"string\"\r\n            },\r\n            \"value\": {\r\n                \"type\": \"string\"\r\n            }\r\n        },\r\n        \"required\": [\r\n            \"from\",\r\n            \"op\",\r\n            \"path\",\r\n            \"value\"\r\n        ]\r\n    }\r\n} ."),
            //    new ChatMessage(ChatRole.User, $"{input}"),

            //},
            //            MaxTokens = 1000
            //        };

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
    {
        new ChatMessage(ChatRole.System, "You are an assistant that provides Azure DevOps work items in a simple JSON format."),        
        //new ChatMessage(ChatRole.System, "Each work item should be a array consisting of objects with fields 'op',  'title', and 'description'. An example of an valid object is {\"id\": 1, \"title\": \"Sample Task\", \"description\": \"This is a sample description.\"} "),
        new ChatMessage(ChatRole.System, "Format each work item as an array containing at least one object. Each object must have the keys 'from', 'op', 'path', and 'value'. "),
        new ChatMessage(ChatRole.System, "For the Title object Set 'from' to null, 'op' to 'add', and 'path' to '/fields/System.Title'. The 'value' should contain the task's title. For example: [{\"from\": null, \"op\": \"add\", \"path\": \"/fields/System.Title\", \"value\": \"Sample task\"}]"),
        new ChatMessage(ChatRole.System, "For the Description object Set 'from' to null, 'op' to 'add', and 'path' to '/fields/System.Description'. The 'value' should contain the task's description."),
        new ChatMessage(ChatRole.System, "You should provide each workitem with an Title and corresponding description"),       
        new ChatMessage(ChatRole.System, "the arrays containing the individual workitems should be collected in an array using square brackets [] and given the name workitems. An example of an valid array output is \"workitems\": [[{object1}],[{object2}], etc..]"),
        new ChatMessage(ChatRole.System, "the data should be enclosed in {} as in JSON format. and example is {\"workitems\":[[{object1}],[{object2}]}"),
        new ChatMessage(ChatRole.User, $"{input}"),
    },
                MaxTokens = 1000
            };




            Response<ChatCompletions> response = client.GetChatCompletions(
                deploymentOrModelName: "gpt-35-turbo",
                chatCompletionsOptions);



            Console.WriteLine("Response content before json extract");
            Console.WriteLine(response.Value.Choices[0].Message.Content);

            
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
