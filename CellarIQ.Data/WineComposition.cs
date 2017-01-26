using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace CellarIQ.Data
{
    public class WineComposition
    {
        [JsonProperty("varietal_name")]
        public string VarietalName { get; set; }

        [BsonElement("color")]
        public string Color { get; set; }

        [JsonProperty("percent_composition")]
        public Decimal PercentComposition { get; set; }
    }
}