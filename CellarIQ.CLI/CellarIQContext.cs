using System;
using CellarIQ.Data;
using MongoDB.Driver;

namespace CellarIQ.CLI
{
    internal class CellarIQContext
    {
        public IMongoDatabase Database;


        public CellarIQContext(string connectionString)
        {
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase("CellarIQ");

        }

        public IMongoCollection<Wine> Wine
            => Database.GetCollection<Wine>("Wine");
    }
}