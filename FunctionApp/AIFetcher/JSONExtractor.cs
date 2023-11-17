using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIFetcher
{
    internal class JSONExtractor
    {
        public static JObject ExtractJson(string response)
        {
            int startIndex = response.IndexOf('{');
            int endIndex = response.LastIndexOf('}') + 1; // +1 to include the closing brace

            string jsonPart = "";
            try
            {
                if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
                {
                    jsonPart = response.Substring(startIndex, endIndex - startIndex);
                    // Now jsonPart contains the JSON string
                    

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
            return JObject.Parse(jsonPart);
        }
    }
}


















