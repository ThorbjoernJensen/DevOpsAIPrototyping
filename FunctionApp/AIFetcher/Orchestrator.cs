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

namespace AIFetcher
{
    public class Orchestrator
    {
        

        [FunctionName("OrchestrationProcesser")]
        public async Task<string> ProcessOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            Console.WriteLine("Orch proces calledl");
            Console.WriteLine("context.instanceId fra OrchProcesser: "+ context.InstanceId);

            UserInput userInput = context.GetInput<UserInput>();
            //bool running = true;


            var chatCompletionsOptions = await context.CallActivityAsync<ChatCompletionsOptions>("InstantiateMessageHistory",(userInput));

            userInput.input = await context.WaitForExternalEvent<string>("UserInput");
            Console.WriteLine("Recieved id in orchestrator: " + userInput.input);

            chatCompletionsOptions = await context.CallActivityAsync<ChatCompletionsOptions>("UpdateMessageHistory", (userInput, chatCompletionsOptions));


          



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


            return "hello";
        }
    }
}
