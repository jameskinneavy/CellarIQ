using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace CellarIQ.Data
{
    
    [Serializable]
    public class Wine : Resource
    {
        public Wine()
        {
            WineComposition = new List<WineComposition>();
        }
        [JsonProperty("label")]
        public string Label { get; set; }
        
        [JsonProperty("vintner_name")]
        public string VintnerName { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("percent_alcohol")]
         public Decimal PercentAlcohol { get; set; }
        
        [JsonProperty("wine_composition")]
        public List<WineComposition> WineComposition { get; set; }

        [JsonProperty("vintage")]
        public string Vintage { get; set; }

        [JsonProperty("cases_produced")]
        public int CasesProduced { get; set; }
    }
}