using Newtonsoft.Json;

namespace CellarIQ.Data
{
    public class CollectionCount
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("continuationToken")]
        public string ContinuationToken { get; set; }
    }
}