using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AIFetcher.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ChatResponse
    {
        [JsonProperty("value")]
        public JObject CurrentChatResponse { get; set; }

        public void Set(JObject jObject)
        {
            this.CurrentChatResponse = jObject;
        }
        public Task<JObject> Get()
        {
            return Task.FromResult(this.CurrentChatResponse);
        }


        public void Delete()
        {
            Entity.Current.DeleteState();
        }

        [FunctionName(nameof(ChatResponse))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<ChatResponse>();


        public override bool Equals(object obj)
        {
            var other = obj as ChatResponse;

            if (other == null)
                return false;

            // Compare JObject properties for equality using ToString representation
            return CurrentChatResponse.ToString() == other.ToString();
            
            
        }

    }

}





//[JsonObject(MemberSerialization.OptIn)]
//public class Counter
//{
//    [JsonProperty("value")]
//    public int Value { get; set; }

//    public void Add(int amount)
//    {
//        this.Value += amount;
//    }

//    public Task Reset()
//    {
//        this.Value = 0;
//        return Task.CompletedTask;
//    }

//    public Task<int> Get()
//    {
//        return Task.FromResult(this.Value);
//    }

//    public void Delete()
//    {
//        Entity.Current.DeleteState();
//    }

//    [FunctionName(nameof(Counter))]
//    public static Task Run([EntityTrigger] IDurableEntityContext ctx)
//        => ctx.DispatchAsync<Counter>();
//}