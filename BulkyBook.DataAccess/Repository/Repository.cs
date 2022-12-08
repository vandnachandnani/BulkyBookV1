using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private DbSet<T> dbSet;

        public Repository(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
            dbSet = _db.Set<T>();

        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public IEnumerable<T> GetAll(string? includeProperty=null)
        {
            IQueryable<T> iquery=dbSet;
            if(!string.IsNullOrEmpty(includeProperty))
            {
                foreach(var inclu in includeProperty.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    iquery = iquery.Include(inclu);
                }
            }
            return iquery.ToList();
        }
        public IEnumerable<T> GetList(Expression<Func<T, bool>> Filter, string? includeProperty = null)
        {
            IQueryable<T> iquery = dbSet;
            iquery = iquery.Where(Filter);
            if (!string.IsNullOrEmpty(includeProperty))
            {
                foreach (var inclu in includeProperty.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    iquery = iquery.Include(inclu);
                }
            }
            return iquery.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> Filter, string? includeProperty = null )
        {
            IQueryable<T> query;
            
                query= dbSet;   
            
           
            query = query.Where(Filter);
            if (!string.IsNullOrEmpty(includeProperty))
            {
                foreach (var inclu in includeProperty.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(inclu);
                }
            }
            return query.FirstOrDefault();
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }

        
    }
}
