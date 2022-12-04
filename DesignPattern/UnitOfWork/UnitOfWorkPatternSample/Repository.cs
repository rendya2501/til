using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UnitOfWorkPatternSample
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            Context = context;
        }

        public TEntity Get(int id)
        {
            return Context.Set<TEntity>().Find(id);
        }
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().where(predicate);
        }

        public IEnumerable<TEntity> GetAll()
        {
            throw new NotImplementedException();
        }


        public void Add(TEntity entity)
        {

        }
        public void AddRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }



        public void Remove(TEntity entity)
        {
            throw new NotImplementedException();
        }
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }
    }
}