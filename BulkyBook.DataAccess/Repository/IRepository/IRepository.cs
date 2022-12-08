using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        public void Add(T entity);
        public void Delete(T entity);   
        public IEnumerable<T> GetAll(string? includeProperty);
        public IEnumerable<T> GetList(Expression<Func<T, bool>> Filter, string? includeProperty = null);
        public T GetFirstOrDefault(Expression<Func<T, bool>> Filter, string? includeProperty);
        void RemoveRange(IEnumerable<T> entity);
    }
}
