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
using AIFetcher.Entities;

namespace AIFetcher
{
    public class Orchestrator
    {


        [FunctionName("OrchestrationProcesser")]
        public async Task<IActionResult> ProcessOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            Console.WriteLine("Orch proces calledl");
            Console.WriteLine("context.instanceId fra OrchProcesser: " + context.InstanceId);

            UserInput userInput = context.GetInput<UserInput>();
            bool running = true;

            string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            string key = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");

            var chatCompletionsOptions = await context.CallActivityAsync<ChatCompletionsOptions>(nameof(BuildMessageHistory.InstantiateChatCompletions), null);

            while (running)
            {
                //userInput.input = await context.WaitForExternalEvent<string>("UserInput");
                userInput = await context.WaitForExternalEvent<UserInput>("UserInput");
                Console.WriteLine("Event Recieved in orchestrator with userMessage: " + userInput.input);

                ChatMessage chatMessageUserInput = new ChatMessage(ChatRole.User, userInput.input);

                //userChatInput is appended to already existing message history
                chatCompletionsOptions = await context.CallActivityAsync<ChatCompletionsOptions>(nameof(BuildMessageHistory.AppendChatMessage), (chatMessageUserInput, chatCompletionsOptions));
                
                
                //Console.WriteLine("message history i orchestrator:");
                //foreach (var item in chatCompletionsOptions.Messages)
                //{
                //    Console.WriteLine(item.Content);
                //}

                //Response<ChatCompletions> response = await context.CallActivityAsync<Response<ChatCompletions>>(nameof(HandleAIFetch.PostChatRequest), (endpoint, key, chatCompletionsOptions));
                
                string response = await context.CallActivityAsync<string>(nameof(HandleAIFetch.PostChatRequest), (endpoint, key, chatCompletionsOptions));                         

                JObject jObject = await context.CallActivityAsync<JObject>(nameof(HandleAIFetch.CompileResponseObject), response);
                Console.WriteLine("vores objekt til response: " + jObject.ToString(Newtonsoft.Json.Formatting.Indented));

                //await context.CallEntityAsync<ChatResponse>("ChatResponse", nameof(ChatResponse.Set), jObject);
;
                ChatMessage chatMessageAIOutput = new ChatMessage(ChatRole.System, jObject.ToString(Newtonsoft.Json.Formatting.Indented));
                //Updates message history with ai response

                chatCompletionsOptions = await context.CallActivityAsync<ChatCompletionsOptions>(nameof(BuildMessageHistory.AppendChatMessage), (chatMessageAIOutput, chatCompletionsOptions));

                Console.WriteLine("message history i orchestrator efter append af AIChatInput: ");
                foreach (var item in chatCompletionsOptions.Messages)
                {
                    Console.WriteLine(item.Content);
                }
                Console.WriteLine("-end of message history-");

                var entityId = new EntityId(nameof(ChatResponse), userInput.instanceId);

                // Two-way call to the entity which returns a value - awaits the response               
                await context.CallEntityAsync(entityId, "Set", jObject);
                
                JObject currentJObject = await context.CallEntityAsync<JObject>(entityId, "Get");                               
                Console.WriteLine("vores entity efter call i orchestrator: " + currentJObject.ToString(Newtonsoft.Json.Formatting.Indented));

            }




            return new OkObjectResult($"Orchestration with instanceId {context.InstanceId} ended.");

    
            Console.WriteLine("Orchestrator returned");



        }
    }
}
