using System;
using Microsoft.Azure.Documents.Client;

namespace CellarIQ.Data
{
    public class WineLabelRepository : Repository<Wine>
    {
        //public static WineLabelRepository New()
        //{
        //    DocumentClient client = new DocumentClient(new Uri("https://cellariq.documents.azure.com:443/"), "FmnJBTtWCFpKZ89TpAfcuwxrOY3Cs8fIvjuVQsTBGyreCEm4D8GEaCpN4PhCDT8VMT7P1NiY4UxAE7mjORkVCg==");
        //    string db = "cellariq";
        //    string coll = "wine";
        //    return new WineLabelRepository(client, db, coll);
        //}
        public WineLabelRepository(DocumentClient client, string databaseName) : base(client, databaseName, "wine")
        {
        }

        public WineLabelRepository(CellarConfiguation config)
            : this(new DocumentClient(new Uri(config.CellarDatabaseConnectionUri), config.CellarDatabaseKey), config.DatabaseId)
        {
            
        }
    }
}