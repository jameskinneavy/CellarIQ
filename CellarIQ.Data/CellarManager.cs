using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CellarIQ.Data.Utility;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace CellarIQ.Data
{
    [Serializable]
    public class CellarManager
    {
        private readonly CellarConfiguation _config;
        private DocumentClient _client;
        private Repository<Wine> _wineRepo;
        private CellarItemRepository _cellarItemRepo;
        private SearchServiceClient _searchServiceClient;

        public CellarManager(CellarConfiguation config)
        {
            _config = config;
            _client = new DocumentClient(new Uri(config.CellarDatabaseConnectionUri), config.CellarDatabaseKey);

            _wineRepo = new WineLabelRepository(_client, config.DatabaseId);
            _cellarItemRepo = new CellarItemRepository(_client, config.DatabaseId, DocumentCollections.CellarItems);

            _searchServiceClient = new SearchServiceClient(config.SearchServiceName,
                new SearchCredentials(config.SearchServiceKey));

        }

        public async Task DeleteAllWines()
        {
            var all = _wineRepo.GetAll();
            foreach (var one in all)
            {
                await _wineRepo.Remove(one);
            }
        }

        public async Task DeleteAllCellarItems()
        {
            var all = _cellarItemRepo.GetAll();
            foreach (var one in all)
            {
                await _cellarItemRepo.Remove(one);
            }
        }

        public async Task<bool> AddWineLabelIfNotExists(Wine wine)
        {
            if (!WineLabelExists(wine))
            {
                await _wineRepo.Add(wine);
                return false;
            }

            return true;

        }

        public bool WineLabelExists(Wine wineTemplate)
        {
            return _wineRepo.Exists(w => w.VintnerName.ToLower().Equals(wineTemplate.VintnerName.ToLower())
                                         && w.Vintage.Equals(wineTemplate.Vintage)
                                         && w.Label.ToLower().Equals(wineTemplate.Label.ToLower()));
        }
        public Wine GetWine(Wine wineTemplate)
        {
            var wines = _wineRepo.Find(
                        w => w.VintnerName.ToLower().Equals(wineTemplate.VintnerName.ToLower())
                        && w.Vintage.Equals(wineTemplate.Vintage)
                        && w.Label.ToLower().Equals(wineTemplate.Label.ToLower()));

            return wines.SingleOrDefault();

        }
        
        public IEnumerable<Wine> GetAllWineLabels()
        {
            return _wineRepo.GetAll();
        }

        public async void AddCellarItem(CellarItem cellarItem)
        {
            await _cellarItemRepo.Add(cellarItem);
        }

        public IEnumerable<CellarItem> GetAllCellarItems()
        {
            var wineDict = GetDictionaryOfWines();

            var cellarItems = _cellarItemRepo.GetAll().ToList();
            foreach (var cellarItem in cellarItems)
            {
                cellarItem.WineInfo = wineDict[cellarItem.WineId];
            }

            return cellarItems;
        }

        private Dictionary<string, Wine> GetDictionaryOfWines()
        {
            List<Wine> allWines = GetAllWineLabels().ToList();
            Dictionary<string, Wine> wineDict = new Dictionary<string, Wine>();
            allWines.ForEach(w => wineDict.Add(w.Id, w));
            return wineDict;
        }

        public IEnumerable<Wine> FindWineLabelsByVintner(string vintnerName)
        {
            return _wineRepo.Find(w => w.VintnerName.ToLower().Contains(vintnerName.ToLower()));
        }

        public DocumentClient GetClient()
        {
            return _client;
        }

        public IEnumerable<Wine> FindWines(WineSearchParameters wineSearchParams)
        {
            List<string> ids = ExecuteTextSearchOnWineDescription(wineSearchParams);

            // Get the wines from the document database with those ids
            IEnumerable<Wine> matchingWines;
            if (ids.Count > 0)
            {
                Expression<Func<Wine, bool>> wineSearchPredicate = w => ids.Contains(w.Id);
                matchingWines = _wineRepo.Find(wineSearchPredicate);

                if (wineSearchParams.Vintner != null)
                {
                    matchingWines =
                        matchingWines.Where(w => w.VintnerName.ToLower().Contains(wineSearchParams.Vintner.ToLower()));
                }
            }
            else
            {
                matchingWines = _wineRepo.GetAll();
                if (wineSearchParams.Vintner != null)
                {
                    matchingWines = matchingWines.Where(w => w.VintnerName.ToLower().Contains(wineSearchParams.Vintner.ToLower()));
                }

                if (wineSearchParams.Vintage != null)
                {
                    matchingWines = matchingWines.Where(w => w.Vintage.Equals(wineSearchParams.Vintage));
                }

            }

            return matchingWines.ToList();
        }
        public IEnumerable<CellarItem> FindCellarItems(WineSearchParameters wineSearchParams)
        {
            IEnumerable<Wine> wines = _wineRepo.GetAll().ToList();

            List<string> ids = ExecuteTextSearchOnWineDescription(wineSearchParams);

            List<CellarItem> matchingCellarItems;
            if (ids.Count > 0)
            {
                matchingCellarItems = _cellarItemRepo.Find(ci => ids.Contains(ci.WineId)).ToList();
                foreach (var cellarItem in matchingCellarItems)
                {
                    cellarItem.WineInfo = wines.Single(w => w.Id == cellarItem.WineId);
                }

                // Filter on vintner
                if (wineSearchParams.Vintner != null)
                {
                    matchingCellarItems =
                        matchingCellarItems.Where(
                            ci =>
                                ci.WineInfo.VintnerName.Equals(wineSearchParams.Vintner,
                                    StringComparison.OrdinalIgnoreCase))
                            .ToList();
                }
            }
            else
            {

                if (wineSearchParams.Vintner != null)
                {
                    wines = wines.Where(w => w.VintnerName.ToLower().Contains(wineSearchParams.Vintner.ToLower()));
                }

                if (wineSearchParams.Vintage != null)
                {
                    wines = wines.Where(w => w.Vintage.Equals(wineSearchParams.Vintage));
                }
                ids.AddRange(wines.Select(w => w.Id));

                matchingCellarItems = _cellarItemRepo.Find(ci => ids.Contains(ci.WineId)).ToList();

                foreach (var cellarItem in matchingCellarItems)
                {
                    cellarItem.WineInfo = wines.Single(w => w.Id == cellarItem.WineId);
                }

            }

            // Add location search elements
            if (wineSearchParams.StorageUnit != null)
            {
                Expression<Func<CellarItem, bool>> cellarItemPredicate = PredicateBuilder.True<CellarItem>();

                cellarItemPredicate =
                    cellarItemPredicate.And(
                        ci => ci.StorageUnit.Equals(wineSearchParams.StorageUnit, StringComparison.OrdinalIgnoreCase));

                if (wineSearchParams.Shelf != null)
                {
                    cellarItemPredicate =
                        cellarItemPredicate.And(
                            ci => ci.StorageShelf.Equals(wineSearchParams.Shelf, StringComparison.OrdinalIgnoreCase));
                }

                // Check if prior query returned any results - generally not
                matchingCellarItems = !matchingCellarItems.Any() 
                    ? _cellarItemRepo.Find(cellarItemPredicate).ToList() 
                    : matchingCellarItems.Where(cellarItemPredicate.Compile()).ToList();
                
            }
            
            // Sort for later
            matchingCellarItems.Sort(CompareCellarItems);

            return matchingCellarItems;
        }

        private List<string> ExecuteTextSearchOnWineDescription(WineSearchParameters wineSearchParams)
        {
            List<string> ids = new List<string>();

            // Only execute search if there is something provided
            if (wineSearchParams.WineDescription == null && wineSearchParams.WineLabel == null)
            {
                return ids;
            }

            ISearchIndexClient indexClient = _searchServiceClient.Indexes.GetClient("wine");

            IList<string> searchFields = new List<string> { "label" };

            List<string> filterTerms = new List<string>();
            if (wineSearchParams.Vintage != null)
            {
                filterTerms.Add($"vintage eq '{wineSearchParams.Vintage}'");
            }

            searchFields.Add("vintner");


            string filter = string.Empty;
            if (filterTerms.Count > 0)
            {
                foreach (string filterTerm in filterTerms)
                {
                    filter += $"and {filterTerm}";
                }

                filter = filter.Substring(4);
            }


            var searchParams = new SearchParameters()
            {
                Select = new[] { "id" },
                SearchFields = searchFields,
                Filter = filter,
                QueryType = QueryType.Full
            };

            // Use Azure serch to find the ids of the wines we want
            DocumentSearchResult<WineSearchResult> results =
                indexClient.Documents.Search<WineSearchResult>(wineSearchParams.WineDescription + " " + wineSearchParams.WineLabel, searchParams);

            // Get the list of cellar items for those wines
            ids.AddRange(results.Results.Select(r => r.Document.Id));
            return ids;
        }

        private int CompareCellarItems(CellarItem cellarItem1, CellarItem cellarItem2)
        {
            string cellarItemData1 = $"{cellarItem1.WineInfo.VintnerName} {cellarItem1.WineInfo.Vintage} {cellarItem1.WineInfo.Label} {cellarItem1.StorageUnit} {cellarItem1.StorageShelf}";
            string cellarItemData2 = $"{cellarItem2.WineInfo.VintnerName} {cellarItem2.WineInfo.Vintage} {cellarItem2.WineInfo.Label} {cellarItem1.StorageUnit} {cellarItem1.StorageShelf}";

            return string.Compare(cellarItemData1, cellarItemData2, StringComparison.Ordinal);
        }

    }
}
