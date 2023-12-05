using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;

namespace AIFetcher
{
    internal class JSONExtractor
    {
        public static JObject ExtractJson(string responseWithTicks)
        {
            string response = responseWithTicks.Replace("```", "");

            int JSONstartIndex = response.IndexOf('{');
            int JSONendIndex = response.LastIndexOf('}') + 1; // +1 to include the closing brace

            string chatPart = "";
            string jsonPart = "";
            JObject json = new JObject();

            string cleanedResponse = response.Replace("```json", "");
            cleanedResponse = response.Replace("```", "");
            
            try
            {
                if (JSONstartIndex != -1 && JSONendIndex != -1 && JSONendIndex > JSONstartIndex)
                {
                    jsonPart = response.Substring(JSONstartIndex, JSONendIndex - JSONstartIndex);

                    json = JObject.Parse(jsonPart); 

                    chatPart = response.Substring(0, JSONstartIndex);
                    // Now jsonPart contains the JSON string
                    // 
                    json["message"] = chatPart;         
                }
                else
                {
                    // Handle cases where the braces are not found
                }

            }
            catch (JsonReaderException)
            {
                throw new InvalidOperationException("Invalid JSON data.");
            }
            return json;
        }
    }
    //public class ResponseObject
    //{
    //    public string chatMessage { get; set; }
    //    public List<string> workItems { get; set; }
    //}
}

//ResponseObject jsonResponseObject = new ResponseObject
//{
//    name = endpointName,
//    removed = attributesRemoved,
//    added = attributesAdded
//};

//string json = JsonConvert.SerializeObject(jsonResponseObject);

////Console.WriteLine(json);
//return new OkObjectResult(json);
//        }
//    }
//    public class ResponseObject
//{
//    public string name { get; set; }
//    public List<string> removed { get; set; }
//    public List<string> added { get; set; }
//}


















