using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CellarIQ.Data;
using CsvHelper;

namespace CellarIQ.CLI.Commands
{
    internal class LoadCellarItemsFromCsv : CellarCommand
    {
        public LoadCellarItemsFromCsv(CellarManager manager) : base(manager)
        {
        }

        public override string Execute(string[] args)
        {
            string path = args[0];
            int itemsLoaded = 0;
            StringBuilder errorBuilder = new StringBuilder();

            TextReader reader = File.OpenText(path);
            using (CsvReader csvReader = new CsvReader(reader))
            {
                while (csvReader.Read())
                {
                    try
                    {
                        CellarItem cellarItem = GetCellarItem(csvReader);
                        CellarManager.AddCellarItem(cellarItem);
                        itemsLoaded += 1;
                        

                    }
                    catch (Exception e)
                    {

                        errorBuilder.AppendFormat("\nCould not load '{0}' because {1}",
                            string.Join(",", csvReader.CurrentRecord), e.Message);
                    }
                }
            }

            return $"Loaded {itemsLoaded} cellar items into inventory.{errorBuilder}";
        }

        private CellarItem GetCellarItem(CsvReader csvReader)
        {
            CellarItem result = null;

            string vintner = csvReader.GetField<string>(0);
            string vintage = csvReader.GetField<string>(1);
            string label = csvReader.GetField<string>(2);

            Wine wineQ = new Wine
            {
                VintnerName = vintner,
                Vintage = vintage,
                Label = label
            };

            Wine wine = CellarManager.GetWine(wineQ);
            if (wine != null)
            {
                result = new CellarItem();
                result.WineId = wine.Id;
                result.StorageUnit = csvReader.GetField<string>(5);
                result.StorageShelf = csvReader.GetField<string>(6);
            }
            return result;
        }
    }
}