using System;
using CellarIQ.Common;
using CellarIQ.Data;
using CsvHelper;

namespace CellarIQ.CLI.Utilities
{
    internal class WineLabelCsvHelper
    {
        public static Wine GetWine(CsvReader csvReader)
        {
            string vintner = csvReader.GetField<string>(0);
            string vintage = csvReader.GetField<string>(1);
            string label = csvReader.GetField<string>(2);
            string comp = csvReader.GetField<string>(3).Trim();

            decimal abv = 0;
            bool hasAbv = csvReader.TryGetField<decimal>(4, out abv);

            int casesProduced = 0;
            bool hasCasesProduced = csvReader.TryGetField<int>(5, out casesProduced);
            

            Wine wine = new Wine
            {
                Label = label,
                VintnerName = vintner,
                Vintage = vintage,
                PercentAlcohol = abv,
                CasesProduced = casesProduced,
                Color = csvReader.GetField<string>(6)
            };

           
            string[] components = comp.Split(';');
            foreach (string component in components)
            {
                if (component.Length > 0)
                {
                    
                    string[] compDetails = component.Trim().Split(new[] {' '}, 2);
                    WineComposition c = new WineComposition
                    {
                        VarietalName = compDetails[1].Trim(),
                        PercentComposition = Convert.ToDecimal(compDetails[0].Trim())
                    };
                    c.Color = ColorMapper.GetVarietalColor(c.VarietalName);
                    wine.WineComposition.Add(c);
                    
                }
                    
            }
            

            return wine;
        }
    }
}