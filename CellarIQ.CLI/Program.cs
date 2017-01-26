using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CellarIQ.CLI.Commands;
using CellarIQ.Data;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace CellarIQ.CLI
{
    class Program
    {

        private CellarManager _cellarManager;


        static void Main(string[] args)
        {

            string x = null;
            string y = "hello";

            string z = x + y;

            Program p = new Program();
            try
            {
                p.InitializeCellarManager();
                p.Run();

            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                Thread.Sleep(10000);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
                Thread.Sleep(19000);
            }


        }

        private void Run()
        {

    //        foreach (var family in client.CreateDocumentQuery(collectionLink,
    //"SELECT * FROM Families f WHERE f.id = \"AndersenFamily\""))
    //        {
    //            Console.WriteLine("\tRead {0} from SQL", family);
    //        }

    //        SqlQuerySpec query = new SqlQuerySpec("SELECT * FROM Families f WHERE f.id = @familyId");
    //        query.Parameters = new SqlParameterCollection();
    //        query.Parameters.Add(new SqlParameter("@familyId", "AndersenFamily"));

    //        foreach (var family in client.CreateDocumentQuery(collectionLink, query))
    //        {
    //            Console.WriteLine("\tRead {0} from parameterized SQL", family);
    //        }

            


            StringBuilder documentBuilder = new StringBuilder("[");
            
            var client = _cellarManager.GetClient();
            foreach (var wine in client.CreateDocumentQuery(
                UriFactory.CreateDocumentCollectionUri("cellariq", "wine"),
                "SELECT w.id, w.vintage, w.vintner_name as vintner, w.label, w.wine_composition as components, w.percent_alcohol as percentAlcohol FROM Wines w"))
            {
                //Console.WriteLine(wine);
                JObject core = JObject.Parse(wine.ToString());
                core.Add("@search.action", "upload");
                core.Remove("components");
                
                JArray components = wine.components;
                if (components.Count > 0)
                {
                    var varietals = components.SelectTokens("..varietal_name").Select(jt => jt.Value<string>()).ToList();
                    JToken t = new JArray(varietals);
                    core.Add("components", t);

                    //Console.WriteLine(varietals);
                }
                documentBuilder.AppendFormat("{0},\n", core);
                
            }

            documentBuilder.Length -= 2;
            documentBuilder.Append("]");

            string path = "c:\\temp\\unique-wines.text";
           
            File.WriteAllText(path, documentBuilder.ToString());

            Console.WriteLine("done");
            Console.ReadLine();
            return;

            CommandProcessor processor = new CommandProcessor(_cellarManager);
            bool exit = false;
            while (!exit)
            {
                Console.Write("Enter a command: ");
                string command = Console.ReadLine();

                if (command != null && command.ToLower() != "quit")
                {
                    string result = processor.Execute(command);
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine("Exiting system");
                    Thread.Sleep(1500);
                    exit = true;
                }


            }
        }

        private void InitializeCellarManager()
        {
            CellarConfiguation config = new CellarConfiguation();
            config.CellarDatabaseConnectionUri = "https://cellariq.documents.azure.com:443/";
            config.CellarDatabaseKey = "FmnJBTtWCFpKZ89TpAfcuwxrOY3Cs8fIvjuVQsTBGyreCEm4D8GEaCpN4PhCDT8VMT7P1NiY4UxAE7mjORkVCg==";
            config.DatabaseId = "cellariq";
            _cellarManager = new CellarManager(config);
        }
        private async Task<int> GetCount(Repository<Wine> repo)
        {
            return await repo.Count();
        }
        private async Task AddWine(Repository<Wine> repo, Wine w)
        {
            await repo.Add(w);

        }

        private async Task ShowDocuments()
        {
            DocumentClient client = new DocumentClient(new Uri("https://cellariq.documents.azure.com:443/"), "FmnJBTtWCFpKZ89TpAfcuwxrOY3Cs8fIvjuVQsTBGyreCEm4D8GEaCpN4PhCDT8VMT7P1NiY4UxAE7mjORkVCg==");

            IQueryable<Wine> wines =
                client.CreateDocumentQuery<Wine>(UriFactory.CreateDocumentCollectionUri("cellariq", "wine"));

            foreach (var wine in wines)
            {
                Console.Write(wine.Label + ": ");

                foreach (var comp in wine.WineComposition)
                {
                    Console.Write($"({comp.VarietalName}, {comp.Color}, {comp.PercentComposition}),");
                }
                Console.WriteLine();
            }

        }
        private async Task LoadDocumentsFromFile()
        {

            DocumentClient client = new DocumentClient(new Uri("https://cellariq.documents.azure.com:443/"), "FmnJBTtWCFpKZ89TpAfcuwxrOY3Cs8fIvjuVQsTBGyreCEm4D8GEaCpN4PhCDT8VMT7P1NiY4UxAE7mjORkVCg==");
            string file = "c:\\temp\\cellar-items.txt";

            string data = File.ReadAllText(file);

            string[] result = Regex.Split(data, "\r\n},");
            int docId = 0;
            foreach (string doc in result)
            {
                var completeDoc = doc + "\r\n}";
                File.WriteAllText("c:\\temp\\cellaritems\\cellar-item-" + ++docId + ".json", completeDoc);
            }

            // Console.WriteLine(result);


            //MatchCollection matches = Regex.Matches(data, "{\r\n\"wine_id\":.+?]\r\n}");

            //int matchCount= matches.Count;

            //int docId = 0;
            //foreach (Match match in matches)
            //{
            //    //docId++;
            //    string doc = match.Value;
            //    File.WriteAllText("c:\\temp\\cellaritems\\cellar-item-" + ++docId +".json", doc);
            //}


        }

        static void Mainx(string[] args)
        {
            ////string conn = ConfigurationManager.AppSettings["config-mongo-local"];

            ////CellarIQContext ctxFrom = new CellarIQContext(conn);

            string conn2 = ConfigurationManager.AppSettings["config-mongo-azure"];

            CellarIQContext ctx2 = new CellarIQContext(conn2);



            List<Wine> wines = ctx2.Wine.AsQueryable().Where(w => w.Label.Contains("Red")).ToList();

            //IndexKeysDefinition<Wine> ik = Builders<Wine>.IndexKeys.Text(f => f.WineLabel);

            //ctx2.Wine.Indexes.CreateOne(ik);

            //var result = ctx2.Wine.Find(Builders<Wine>.Filter.Text("Grenache")).ToList();

            ////ctx2.Wine.InsertMany(wines);

            foreach (var wine in wines)
            {


                Console.WriteLine("{0}: {1}", wine.VintnerName, wine.Label);
            }


            //Console.ReadLine();
        }
    }
}
