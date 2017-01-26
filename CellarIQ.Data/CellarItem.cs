using System;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CellarIQ.Data
{
    [Serializable]
    public class CellarItem : Resource
    {
        public CellarItem()
        {
            Container = "Standard";
            
        }

        [JsonProperty("wine_id")]
        public string WineId { get; set; }

        [JsonProperty("acquisition_date")]
        public DateTime AcquisitionDate { get; set; }

        [JsonProperty("acquisition_price")]
        public Decimal AcquisitionPrice { get; set; }

        [JsonProperty("disposal_method")]
        public string DisposalMethod { get; set; }

        [JsonProperty("disposal_date")]
        public DateTime DisposalDate { get; set; }

        [JsonProperty("container")]
        public string Container { get; set; }

        [JsonProperty("storage_unit")]
        public string StorageUnit { get; set; }

        [JsonProperty("storage_shelf")]
        public string StorageShelf { get; set; }

        [JsonProperty("WineInfo")]
        public Wine WineInfo { get; set; }
    }
}