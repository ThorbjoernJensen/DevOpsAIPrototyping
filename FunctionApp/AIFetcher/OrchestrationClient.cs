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
using System.Net;
using AIFetcher.Entities;
using System.ComponentModel;
using System.Xml.Linq;

namespace AIFetcher
{
    internal class OrchestrationClient
    {

       [FunctionName(nameof(OrchestrationStarter))]

       //Starts an orchestration instance and sends the instance id to the frontend
        public async Task<IActionResult> OrchestrationStarter(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {
            log.LogInformation("C# HTTP trigger in OrchestrationStarter function processed a request.");
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

            //Return instanceId instead  
            return starter.CreateCheckStatusResponse(req, instanceId);
            //return new OkObjectResult(json);
        }

        [FunctionName(nameof(OrchestrationInstanceClient))]

        //Serves as an interface between orchestration instance frontend, handling reqests and responses
        public async Task<IActionResult> OrchestrationInstanceClient(
 [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, [DurableClient] IDurableOrchestrationClient client, [DurableClient] IDurableEntityClient entityClient, ILogger log)
        {
            log.LogInformation("C# HTTP trigger in OrchestrationInstanceClient processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            UserInput userInput= JsonConvert.DeserializeObject<UserInput>(requestBody);
            //string userInput = data?.userInput;
            //string instanceId = data?.instanceId;

            Console.WriteLine($"Instance ID: {userInput.instanceId}");

            string entityKey = userInput.instanceId;


            var entityId = new EntityId(nameof(ChatResponse), entityKey);
            EntityStateResponse<JObject> stateResponse1 = await entityClient.ReadEntityStateAsync<JObject>(entityId);
            EntityStateResponse<JObject> stateResponse2;
            Console.WriteLine($"stateResponse1 fetched: {stateResponse1.EntityState}");
            Console.WriteLine($"entity state for stateResponse2 ikke defineret endnu");

            //sends the input to the running orchestration instance
            await client.RaiseEventAsync(userInput.instanceId, "UserInput", userInput);


            // Polling loop that awaits that a new stateResponse has arrived
            while (true)
            {
                Console.WriteLine("inside the client while-loop checking for equality");
                // Query entity state
                stateResponse2 = await entityClient.ReadEntityStateAsync<JObject>(entityId);
                //Console.WriteLine($"jObject entitystate2 fra client while-loop: {stateResponse2.EntityState}");

                Console.WriteLine("StateReponse2 fetched states we compare: ");
                Console.WriteLine("stateResponse1.EntityState:");
                Console.WriteLine(stateResponse1.EntityState);
                Console.WriteLine("stateResponse2.EntityState:");
                Console.WriteLine(stateResponse2.EntityState);


                // Check if the state of the entity has changed
                //If they are not equal - enter the break that returns object result
                if (!JToken.DeepEquals(stateResponse1.EntityState, stateResponse2.EntityState))

                {
                    Console.WriteLine("we entered the condition that !stateResponse1.Equals stateResponse2 - the two Entity states differ");
                    Console.WriteLine("JToken.DeepEquals(stateResponse1.EntityState, stateResponse2.EntityState) = " + JToken.DeepEquals(stateResponse1.EntityState, stateResponse2.EntityState));


                    if (JToken.DeepEquals(stateResponse1.EntityState, stateResponse2.EntityState))
                    {
                        Console.WriteLine("but JToken can see that they are equal");
                    }


                    break;
                }

                Console.WriteLine("Objects were eveluated to be equal: stateResponse1.Equals(stateResponse2) = "+ stateResponse1.Equals(stateResponse2));
             
                Console.WriteLine("JToken.DeepEquals(stateResponse1.EntityState, stateResponse2.EntityState) = "+ JToken.DeepEquals(stateResponse1.EntityState, stateResponse2.EntityState));

                // Wait for a specified interval before polling again
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            Console.WriteLine("The line before response return - the object differed");
            //return new OkObjectResult($"Response from OrchestrationINstanceClient. InstanceId: {userInput.instanceId}");
            return new OkObjectResult(stateResponse2);
        }

    }
}
