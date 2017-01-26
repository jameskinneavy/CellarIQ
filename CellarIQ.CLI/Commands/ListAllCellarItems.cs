using System.Linq;
using System.Text;
using CellarIQ.Data;

namespace CellarIQ.CLI.Commands
{
    internal class ListAllCellarItems : CellarCommand
    {
        public ListAllCellarItems(CellarManager manager) : base(manager)
        {
        }

        public override string Execute(string[] args)
        {
            var cellarItems = CellarManager.GetAllCellarItems().ToList();
            cellarItems.Sort(CompareWines);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"There are {cellarItems.Count} cellar items:");
            foreach (CellarItem cellarItem in cellarItems)
            {
                sb.AppendFormat("{0} {1} {2} (Storage Unit: {3}, Shelf {4} )\n", 
                    cellarItem.WineInfo.VintnerName, cellarItem.WineInfo.Vintage, cellarItem.WineInfo.Label, cellarItem.StorageUnit, cellarItem.StorageShelf);
            }

            return sb.ToString();
        }

        private int CompareWines(CellarItem x, CellarItem y)
        {
            string xdata = $"{x.WineInfo.VintnerName} {x.WineInfo.Vintage} {x.WineInfo.Label} {x.StorageShelf} {x.StorageUnit}";
            string ydata = $"{y.WineInfo.VintnerName} {y.WineInfo.Vintage} {y.WineInfo.Label} {x.StorageShelf} {x.StorageUnit}";

            return xdata.CompareTo(ydata);
        }
    }
}