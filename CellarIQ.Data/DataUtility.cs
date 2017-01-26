using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellarIQ.Data
{
    public class DataUtility
    {
        public static Dictionary<Wine, IEnumerable<CellarItem>> GroupCellarItemsByWine(IEnumerable<CellarItem> cellarItemList)
        {

            Dictionary<Wine, IEnumerable<CellarItem>> cellarItemMap = new Dictionary<Wine, IEnumerable<CellarItem>>();

            List<Wine> wines = cellarItemList.Select(ci => ci.WineInfo).ToList();
            var wineGroup = wines.GroupBy(w => w.Id);

            wines = wineGroup.Select(g => g.First()).ToList();
            
            //IEnumerable<Wine> wines = cellarItemList.Select(ci => ci.WineInfo).Distinct();
            foreach (Wine wine in wines)
            {
                List<CellarItem> cellarItems = cellarItemList.Where(ci => ci.WineId.Equals(wine.Id)).ToList();
                cellarItemMap.Add(wine, cellarItems);
            }
            return cellarItemMap;
        }
    }

}
