using AIFetcher.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AIFetcher
{
    internal class OrchestrationClient
    {

       [FunctionName(nameof(OrchestrationStarter))]
        public async Task<IActionResult> OrchestrationStarter(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            Console.WriteLine("C# HTTP trigger function processed a request.");

            //string invocationId = await starter.StartNewAsync("OrchestrationProcesser", null);

            string input = req.Query["input"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserInput userInput = JsonConvert.DeserializeObject<UserInput>(requestBody);
            //userInput = userInput ?? userInput?.input;

            string instanceId = await starter.StartNewAsync("OrchestrationProcesser", null, userInput);

            string responseMessage = string.IsNullOrEmpty(userInput.input)
                ? $"This HTTP triggered function executed successfully. No input given. here is the instance id: {instanceId}."
                : $"Hello. This HTTP triggered function executed successfully. input: {userInput} here is the instance id: {instanceId}.";

            JObject json = new JObject()
            {
                ["message"] = responseMessage,
                ["instanceId"] = instanceId,
            };

            Console.WriteLine("instanceId fra Orchestrationstarter: " + instanceId);
            Console.WriteLine("console was here (Orchestrator lige før return");

            return starter.CreateCheckStatusResponse(req, instanceId);
            //return new OkObjectResult(json);
        }



    }
}
