using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using AIFetcher.Models;
using Azure;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Security;
using Google.Protobuf.WellKnownTypes;

namespace AIFetcher.ActivityFunctions
{
    internal class HandleAIFetch
    {
        [FunctionName(nameof(PostChatRequest))]
        public async Task<string> PostChatRequest([ActivityTrigger] IDurableActivityContext context, ILogger log)
        //public async Task<Response<ChatCompletions>> PostChatRequest([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            //// Note: The Azure OpenAI client library for .NET is in preview.
            //// Install the .NET library via NuGet: dotnet add package Azure.AI.OpenAI --version 1.0.0-beta.5

            Console.WriteLine("Everything fine ln 27");
            (string endpoint, string key, ChatCompletionsOptions chatCompletionsOptions) = context.GetInput<(string, string, ChatCompletionsOptions)>();
            //ChatCompletionsOptions chatCompletionsOptions = context.GetInput<ChatCompletionsOptions>();
            Console.WriteLine("Everything fine ln 30");
            //string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            //string key = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");

            OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));

            Console.WriteLine("Everything fine ln32");

            Console.WriteLine("message history in PostChatRequest:");
            foreach (var item in chatCompletionsOptions.Messages)
            {
                Console.WriteLine(item.Content);
            }

            Response<ChatCompletions> response = client.GetChatCompletions(
               deploymentOrModelName: "gpt-35-turbo",
               chatCompletionsOptions);

            Console.WriteLine("Response content before json extract");
            Console.WriteLine(response.Value.Choices[0].Message.Content);

            return response.Value.Choices[0].Message.Content;   

        }

        [FunctionName(nameof(CompileResponseObject))]
        public async Task<JObject> CompileResponseObject([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            Console.WriteLine("CompileReponseObject før Response -get");

            string responseRawString = context.GetInput<string>();
            //Response<ChatCompletions> rResponse = context.GetInput<Response<ChatCompletions>>();

            //string responseRawString = rResponse.Value.Choices[0].Message.Content;

            //To only the word json when it comes with ```json
            string response = responseRawString.Replace("```json", "");
            //to exclude remaining ticks
            string cleanedResponse = response.Replace("```", "");
                                             
            //Todo: make prompting that makes AI respond with differentiated json-object with message- and workItem array as properties
            int JSONstartIndex = response.IndexOf('{');
            int JSONendIndex = response.LastIndexOf('}') + 1; // +1 to include the closing brace

            string chatPart = "";
            string jsonPart = "";

            JObject jObject = new JObject();
            Console.WriteLine("CompileReponseObject før try-block");
            try
            {
                if (JSONstartIndex != -1 && JSONendIndex != -1 && JSONendIndex > JSONstartIndex)
                {
                    jsonPart = response.Substring(JSONstartIndex, JSONendIndex - JSONstartIndex);

                    jObject = JObject.Parse(jsonPart);

                    chatPart = response.Substring(0, JSONstartIndex);
                    // Now jsonPart contains the JSON string
                    // 
                    jObject["message"] = chatPart;
                    Console.WriteLine("JObjecten with workitems created");
                }
                //No json-part: we provide an empty workitems object
                else
                {
                    jObject["message"] = response;
                    jObject["WorkItems"] = JObject.Parse("{}");
                    Console.WriteLine("JObjecten with only message created in JSONExtractor");
                }
            }
            catch (JsonReaderException)
            {
                throw new InvalidOperationException("Invalid JSON data.");
            }
            return jObject;
        }
    }
}


