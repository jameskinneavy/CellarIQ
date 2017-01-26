using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CellarIQ.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Resource
    {
        private readonly DocumentClient _client;
        private readonly string _databaseName;
        private readonly string _collectionName;

        public Repository(DocumentClient client, string databaseName, string collectionName)
        {
            _client = client;
            _databaseName = databaseName;
            _collectionName = collectionName;
        }

        public TEntity Get(string id)
        {
            return this.Find(d => d.Id == id).SingleOrDefault();
        }

        public  IEnumerable<TEntity> GetAll()
        {
            var results =
                _client.CreateDocumentQuery<TEntity>(UriFactory.CreateDocumentCollectionUri(_databaseName,
                    _collectionName))
                    .Select(d => d);

            return results;
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return
                _client.CreateDocumentQuery<TEntity>(UriFactory.CreateDocumentCollectionUri(_databaseName,
                    _collectionName)).Where(predicate);
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return
                _client.CreateDocumentQuery<TEntity>(UriFactory.CreateDocumentCollectionUri(_databaseName,
                    _collectionName)).Where(predicate).Any();
        }

        public async Task Add(TEntity entity)
        {
            Uri uri = UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName);
            await _client.CreateDocumentAsync(uri, entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public async Task Remove(TEntity entity)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, entity.Id));
        }

        

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Count()
        {
            Uri uri = UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName);
            string continuationToken = string.Empty;
            int count = 0;
          
            while (continuationToken != null)
            {
                StoredProcedureResponse<CollectionCount> sprocResult = await _client.ExecuteStoredProcedureAsync<CollectionCount>(
                        UriFactory.CreateStoredProcedureUri(_databaseName, _collectionName, "GetCount"), "", continuationToken );
                CollectionCount countRef = sprocResult.Response;
                count += countRef.Count;
                continuationToken = countRef.ContinuationToken;
            }

            return count;

        }

        
    }
}