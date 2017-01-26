using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CellarIQ.CLI;
using CellarIQ.CLI.Commands;
using CellarIQ.CLI.Utilities;
using CellarIQ.Common;
using CellarIQ.Data;
using CsvHelper;

namespace CellarIQ.CLI.Commands
{
    internal class LoadWinesFromCsv : CellarCommand
    {
        public LoadWinesFromCsv(CellarManager manager) : base(manager)
        {
        }

        public override string Execute(string[] args)
        {
            string path = args[0];
            int winesLoaded = 0;
            StringBuilder errorBuilder = new StringBuilder();

            TextReader reader = File.OpenText(path);
            using (CsvReader csvReader = new CsvReader(reader))
            {
                while (csvReader.Read())
                {
                    try
                    {
                        Wine wine = WineLabelCsvHelper.GetWine(csvReader);
                        Task<bool> exists =  CellarManager.AddWineLabelIfNotExists(wine);
                        if (exists.Result == false)
                        {
                            winesLoaded += 1;
                        }
                        else
                        {
                            errorBuilder.AppendFormat("\nCould not load '{0}' because the wine label already exists",
                            string.Join(",", csvReader.CurrentRecord));
                        }
                        
                    }
                    catch (Exception e)
                    {

                        errorBuilder.AppendFormat("\nCould not load '{0}' because {1}",
                            string.Join(",", csvReader.CurrentRecord), e.Message);
                    }
                }
            }

            return $"Loaded {winesLoaded} labels.{errorBuilder}";
        }
    }
}