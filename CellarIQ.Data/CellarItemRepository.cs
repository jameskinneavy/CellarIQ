using Microsoft.Azure.Documents.Client;

namespace CellarIQ.Data
{
    public class CellarItemRepository : Repository<CellarItem>
    {
        public CellarItemRepository(DocumentClient client, string databaseName, string collectionName) : base(client, databaseName, collectionName)
        {
        }
    }
}