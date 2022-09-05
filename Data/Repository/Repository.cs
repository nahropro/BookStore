using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Data.Repository
{
    public class Repository<T> where T:class
    {
        protected readonly BookStoreDbContext BookStoreDbContext;
        private readonly DbSet<T> DbSet;

        public Repository(BookStoreDbContext bookStoreDbContext)
        {
            this.BookStoreDbContext = bookStoreDbContext;
            DbSet = this.BookStoreDbContext.Set<T>();
        }

        protected virtual IQueryable<T> GetQueryable()
        {
            return DbSet.AsQueryable();
        }

        public virtual async Task<long> CountAsync()
        {
            return await DbSet.LongCountAsync();
        }

        public virtual async Task<long> CountAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.LongCountAsync(expression);
        }

        public virtual async Task<T> GetAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<T> GetAsync(long id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<T> GetAsync(string id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual T Get(long id)
        {
            return DbSet.Find(id);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task<List<T>> GetAllNoTrackingAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        public virtual List<T> GetAllNoTracking()
        {
            return DbSet.AsNoTracking().ToList();
        }

        public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.Where(expression).ToListAsync();
        }

        public virtual async Task<List<T>> FindNoTrackingAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.AsNoTracking().Where(expression).ToListAsync();
        }

        public virtual async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.SingleOrDefaultAsync(expression);
        }

        public virtual T SingleOrDefault(Expression<Func<T, bool>> expression)
        {
            return DbSet.SingleOrDefault(expression);
        }

        public virtual async Task<T> SingleOrDefaultNoTrackingAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.AsNoTracking().SingleOrDefaultAsync(expression);
        }

        public virtual T SingleOrDefaultNoTracking(Expression<Func<T, bool>> expression)
        {
            return DbSet.AsNoTracking().SingleOrDefault(expression);
        }

        public virtual T Add(T entity)
        {
            return DbSet.Add(entity);
        }

        public virtual IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            return DbSet.AddRange(entities);
        }

        public virtual T Edit(T entity)
        {
            BookStoreDbContext.Entry<T>(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual T Remove(T entity)
        {
            return DbSet.Remove(entity);
        }

        public virtual async Task<T> RemoveAsync(long id)
        {
            T t =await GetAsync(id);

            if (t != null)
            {
                return Remove(t);
            }

            return null;
        }

        public virtual async Task<T> RemoveAsync(int id)
        {
            T t = await GetAsync(id);

            if (t != null)
            {
                return Remove(t);
            }

            return null;
        }

        public virtual async Task<T> RemoveAsync(string id)
        {
            T t = await GetAsync(id);

            if (t != null)
            {
                return Remove(t);
            }

            return null;
        }

        public virtual IEnumerable<T> RemoveRange(IEnumerable<T> entities)
        {
            return DbSet.RemoveRange(entities);
        }

    }
}