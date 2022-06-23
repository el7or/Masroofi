using Microsoft.EntityFrameworkCore;
using Puzzle.Masroofi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public abstract class RepositoryBase<T> where T : class
    {
        #region Properties

        private MasroofiDbContext dataContext;
        private DbSet<T> dbSet;

        /// <summary>
        /// Entities
        /// </summary>
        protected virtual DbSet<T> Entities
        {
            get
            {
                if (dbSet == null)
                    dbSet = dataContext.Set<T>();
                return dbSet;
            }
        }

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking
        {
            get
            {
                return this.Entities.AsNoTracking();
            }
        }
        #endregion

        public RepositoryBase(MasroofiDbContext dbContext)
        {
            dataContext = dbContext;
            dbSet = dbContext.Set<T>();
        }


        #region Implementation

        public virtual IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }
        public virtual IEnumerable<T> GetAllWhere(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).ToList();
        }
        public virtual IEnumerable<T> GetAllInclude(params Expression<Func<T, object>>[] includes)
        {
            return includes.Aggregate(dbSet.AsQueryable(), (current, include) => current.Include(include));
        }
        public virtual IEnumerable<T> GetAllWhereInclude(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            return includes.Aggregate(dbSet.Where(where), (current, include) => current.Include(include));
        }
        public virtual int GetAllCount()
        {
            return dbSet.Count();
        }
        public virtual Task<int> GetAllCountAsync()
        {
            return dbSet.CountAsync();
        }
        public virtual int GetAllWhereCount(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).Count();
        }
        public virtual Task<int> GetAllWhereCountAsync(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).CountAsync();
        }
        public virtual int GetAllWhereIncludeCount(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            return includes.Aggregate(dbSet.Where(where), (current, include) => current.Include(include)).Count();
        }
        public virtual Task<int> GetAllWhereIncludeCountAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            return includes.Aggregate(dbSet.Where(where), (current, include) => current.Include(include)).CountAsync();
        }

        public virtual T Get(Guid id)
        {
            return dbSet.Find(id);
        }
        public virtual Task<T> GetAsync(Guid id)
        {
            return dbSet.FindAsync(id).AsTask();
        }
        public virtual T Get(string id)
        {
            return dbSet.Find(id);
        }
        public virtual T GetWhere(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).FirstOrDefault<T>();
        }
        public virtual Task<T> GetWhereAsync(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).FirstOrDefaultAsync<T>();
        }
        public virtual T GetWhereInclude(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            return includes.Aggregate(dbSet.Where(where), (current, include) => current.Include(include)).FirstOrDefault<T>();
        }
        public virtual Task<T> GetWhereIncludeAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            return includes.Aggregate(dbSet.Where(where), (current, include) => current.Include(include)).FirstOrDefaultAsync();
        }

        public virtual bool GetAny(Expression<Func<T, bool>> where)
        {
            return dbSet.Any(where);
        }
        public virtual IEnumerable<T> Take(int top, Func<T, object> OrderByColumn)
        {
            return dbSet.OrderByDescending(OrderByColumn).Take(top);
        }

        public virtual T Add(T entity)
        {
            SaveTransaction(entity);
            return dbSet.Add(entity).Entity;
        }
        public virtual void Update(T entity)
        {
            SaveTransaction(entity);
            dbSet.Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;
        }
        public virtual void Delete(T entity)
        {
            SaveTransaction(entity);
            dbSet.Remove(entity);
        }
        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbSet.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
            {
                SaveTransaction(obj);
                dbSet.Remove(obj);
            }
        }
        public virtual bool SaveTransaction(T entity)
        {
            return true;
        }
        #endregion

    }

    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllWhere(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAllInclude(params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetAllWhereInclude(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);

        int GetAllCount();
        int GetAllWhereCount(Expression<Func<T, bool>> where);
        Task<int> GetAllWhereCountAsync(Expression<Func<T, bool>> where);
        int GetAllWhereIncludeCount(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);
        Task<int> GetAllWhereIncludeCountAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);

        T Get(Guid id);
        Task<T> GetAsync(Guid id);
        T Get(string id);
        T GetWhere(Expression<Func<T, bool>> where);
        Task<T> GetWhereAsync(Expression<Func<T, bool>> where);
        T GetWhereInclude(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);
        Task<T> GetWhereIncludeAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);

        bool GetAny(Expression<Func<T, bool>> where);        
        IEnumerable<T> Take(int top, Func<T, object> OrderByColumn);

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        IQueryable<T> TableNoTracking { get; }

        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        bool SaveTransaction(T entity);
    }
}
