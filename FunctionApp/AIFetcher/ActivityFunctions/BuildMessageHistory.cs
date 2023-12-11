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

namespace AIFetcher.ActivityFunctions
{
    internal class BuildMessageHistory
    {
        [FunctionName(nameof(InstantiateChatCompletions))]
        public async Task<ChatCompletionsOptions> InstantiateChatCompletions([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
         
            //Provides basic prompting
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
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

            return chatCompletionsOptions;

        }

        [FunctionName(nameof(AppendUserInput))]
        public async Task<ChatCompletionsOptions> AppendUserInput([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            (UserInput userInput, ChatCompletionsOptions chatCompletionsOptions) = context.GetInput<(UserInput, ChatCompletionsOptions)>();


            //Appends the user input to the message history
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, $"{userInput.input}"));

            return chatCompletionsOptions;



        }

    }


