using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json.Linq;
using Microsoft.Exchange.WebServices.Data;
using AIFetcher.Models;
using System.Collections.Generic;
using System.Data.Common;
using Azure.AI.OpenAI;
using Azure;
using AIFetcher.ActivityFunctions;

namespace AIFetcher
{
    public class Orchestrator
    {
        

        [FunctionName("OrchestrationProcesser")]
        public async Task<IActionResult> ProcessOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            Console.WriteLine("Orch proces calledl");
            Console.WriteLine("context.instanceId fra OrchProcesser: "+ context.InstanceId);

            UserInput userInput = context.GetInput<UserInput>();
            //bool running = true;

            string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            string key = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");

            var chatCompletionsOptions = await context.CallActivityAsync<ChatCompletionsOptions>(nameof(BuildMessageHistory.InstantiateChatCompletions), null);

            userInput.input = await context.WaitForExternalEvent<string>("UserInput");
            Console.WriteLine("Recieved id in orchestrator: " + userInput.input);

            ChatMessage userChatInput = new ChatMessage(ChatRole.User, userInput.input); 

            chatCompletionsOptions = await context.CallActivityAsync<ChatCompletionsOptions>(nameof(BuildMessageHistory.AppendChatMessage), (userChatInput, chatCompletionsOptions));

            Console.WriteLine("message history i orchestrator:");
            foreach (var item in chatCompletionsOptions.Messages)
            {
                Console.WriteLine(item.Content);
            }

            //Response<ChatCompletions> response = await context.CallActivityAsync<Response<ChatCompletions>>(nameof(HandleAIFetch.PostChatRequest), (endpoint, key, chatCompletionsOptions));
            string response = await context.CallActivityAsync<string>(nameof(HandleAIFetch.PostChatRequest), (endpoint, key, chatCompletionsOptions));

            ChatMessage AIChatInput = new ChatMessage(ChatRole.System, userInput.input);

            //Updates message history with ai response
            chatCompletionsOptions = await context.CallActivityAsync<ChatCompletionsOptions>(nameof(BuildMessageHistory.AppendChatMessage), (AIChatInput, chatCompletionsOptions));

            JObject jObject = await context.CallActivityAsync<JObject>(nameof(HandleAIFetch.CompileResponseObject), response);
            Console.WriteLine("vores objekt til response: " + jObject.ToString(Newtonsoft.Json.Formatting.Indented));
            return new OkObjectResult(jObject);
            //





            ////Fetshes and aggregates workItemRelation- and column data
            //(List<WorkItemRelation> workItemRelationList, List<Column> columns) = await context.CallActivityAsync<(List<WorkItemRelation>, List<Column>)>("ProcessMetadata", devOpsRequestData);
            //(UserInput userInput, ChatCompletionsOptions chatCompletionsOptions)


            //while (running)
            //{
            //    //var newPayload = await context.WaitForExternalEvent<string>("newEvent");
            //    var event1 = await context.WaitForExternalEvent<string>("Event1");
            //    Console.WriteLine("Something worked");
            //    // Process the new payload
            //    // ...

            //    if (!running) // Determine if the orchestration should end
            //    {
            //        break;
            //    }
            //}
            Console.WriteLine("something worked");


            
        }
    }
}
