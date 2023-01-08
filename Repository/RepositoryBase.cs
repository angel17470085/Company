using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    { 

        protected RepositoryContext RepositoryContext;
        public RepositoryBase(RepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
            
        }

         public IQueryable<T> FindAll(bool trackChanges)
        {
            // when is set to false we attach the AsNoTracking method to our query to inform ef core
            // that it doesnt need to track changes for the required entities this greatly improves the speed of a query.
            if (!trackChanges)// when is not equal to true 
            {
                return RepositoryContext.Set<T>().AsNoTracking();
            }
            else
            {
                return RepositoryContext.Set<T>();
            }
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
        {
            if  (!trackChanges)  //Not equal to true, meaning its false it will not track changes
            {
                return RepositoryContext.Set<T>().Where(expression).AsNoTracking();
            }
            else
            {
                return RepositoryContext.Set<T>().Where(expression);
            }
        }

        public void Update(T entity)
        {
            RepositoryContext.Set<T>().Update(entity);
        }
        public void Create(T entity)
        {
            RepositoryContext.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            RepositoryContext.Set<T>().Remove(entity); 
        }

    }
}