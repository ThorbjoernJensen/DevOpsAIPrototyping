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

namespace AIFetcher
{
    public class Orchestrator
    {
        

        [FunctionName("OrchestrationProcesser")]
        public async Task<string> ProcessOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            Console.WriteLine("Orch proces calledl");
            Console.WriteLine("context.instanceId fra OrchProcesser: "+ context.InstanceId);

            bool running = true;
            string eventdata= await context.WaitForExternalEvent<string>("Event1");
            Console.WriteLine("Recieved: "+ eventdata);

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
