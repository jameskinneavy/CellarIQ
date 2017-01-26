using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace CellarIQ.Data
{
    [SerializePropertyNamesAsCamelCase]
    public class WineSearchResult
    {
        [Key]
        public string Id { get; set; }
        
        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string Label { get; set; }

        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string Vintner { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        public string[] Components { get; set; }

        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string Vintage { get; set; }

    }
}