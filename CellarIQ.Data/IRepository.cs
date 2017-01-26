using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CellarIQ.Data
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Get(string id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        Task Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        Task Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

        Task<int> Count();
    }
}
