//using Azure.AI.OpenAI;
//using Azure;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Azure;
//using Azure.AI.OpenAI;

//namespace AIFetcher
//{
//    internal class SampleConversation
//    {


//        // Note: The Azure OpenAI client library for .NET is in preview.
//        // Install the .NET library via NuGet: dotnet add package Azure.AI.OpenAI --version 1.0.0-beta.5


//        OpenAIClient client = new OpenAIClient(
//          new Uri("https://oai-devopsaiprototyping.openai.azure.com/"),
//          new AzureKeyCredential(Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")));

//        Response<ChatCompletions> responseWithoutStream = await client.GetChatCompletionsAsync(
//        "gpt-35-turbo",
//        new ChatCompletionsOptions()
//        {
//            Messages =
//          {
//      new ChatMessage(ChatRole.System, @"Assistant is an AI chatbot that helps users turn a natural language list into JSON format. After users input a list they want in JSON format, it will provide suggested list of attribute labels if the user has not provided any, then ask the user to confirm them before creating the list."),
//      new ChatMessage(ChatRole.User, @"hi, could you help me?"),      new ChatMessage(ChatRole.Assistant, @"Of course! I'm here to help. What do you need assistance with?"),      new ChatMessage(ChatRole.User, @"what is your capabilities?"),      new ChatMessage(ChatRole.Assistant, @"I have a variety of capabilities, including:

//1. Answering general knowledge questions
//2. Providing definitions and explanations
//3. Assisting with language translation
//4. Offering suggestions and recommendations
//5. Helping with basic math calculations
//6. Creating and formatting lists in JSON format
//7. Providing weather information
//8. Offering news updates
//9. Assisting with time and date queries
//10. Offering motivational quotes

//Is there anything specific you would like assistance with?"),      new ChatMessage(ChatRole.User, @"I thought you had something to do with JSON?"),      new ChatMessage(ChatRole.Assistant, @"Yes, you're correct! One of my capabilities is helping users turn a natural language list into JSON format. I can assist you in creating a JSON list from the information you provide. Just let me know the details, and I'll guide you through the process."),      new ChatMessage(ChatRole.User, @"what was my first question?"),      new ChatMessage(ChatRole.Assistant, @"Your first question was \"hi, could you help me?\""),
//          },
//            Temperature = (float)0.2,
//            MaxTokens = 350,


//            NucleusSamplingFactor = (float)0.95,
//            FrequencyPenalty = 0,
//            PresencePenalty = 0,
//        });


//        ChatCompletions response = responseWithoutStream.Value;





//    }
//}



