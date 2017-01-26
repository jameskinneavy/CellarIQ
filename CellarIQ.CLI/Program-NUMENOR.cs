using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace CellarIQ.CLI
{
    class Program
    {

        static void Main(string[] args)
        {
            Program p = new Program();
            try
            {
                p.LoadDocumentsFromFile().Wait();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }

        }

        private async Task LoadDocumentsFromFile()
        {

            DocumentClient client = new DocumentClient(new Uri("https://cellariq.documents.azure.com:443/"), "FmnJBTtWCFpKZ89TpAfcuwxrOY3Cs8fIvjuVQsTBGyreCEm4D8GEaCpN4PhCDT8VMT7P1NiY4UxAE7mjORkVCg==");
            string file = "c:\\temp\\cellar-items.txt";

            string data = File.ReadAllText(file);

            MatchCollection matches = Regex.Matches(data, @"({\r\n\t""wine_id"":(.+\s)+(^\t\]$))");

            int matchCount= matches.Count;

            int docId = 0;
            foreach (Match match in matches)
            {
                docId++;
                string doc = match.Value;
                File.WriteAllText("c:\\temp\\winedocs\\cellar-items-{docId}.json", doc);
            }

            
        }

        static void Mainx(string[] args)
        {
            ////string conn = ConfigurationManager.AppSettings["config-mongo-local"];

            ////CellarIQContext ctxFrom = new CellarIQContext(conn);

            string conn2 = ConfigurationManager.AppSettings["config-mongo-azure"];

            CellarIQContext ctx2 = new CellarIQContext(conn2);



            List<Wine> wines = ctx2.Wine.AsQueryable().Where(w => w.Label.Contains("Red")).ToList();

            //IndexKeysDefinition<Wine> ik = Builders<Wine>.IndexKeys.Text(f => f.Label);

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
