using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        DbSet<T> _dbSet;

        public Repository(ApplicationDbContext dbContext) { 
            _context = dbContext;
            _dbSet = dbContext.Set<T>();

            // we can include like this as well to get category
            //_context.Products.Include(obj => obj.category).ToList();
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, String? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(','))
                {
                    query = query.Include(property);
                }
            }

            return query.Where(filter).FirstOrDefault();
        }

        public IEnumerable<T> GetAll(String? includeProperties = null)
        {
            IQueryable<T> values = _dbSet;
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach(var property in includeProperties.Split(','))
                {
                    values = values.Include(property);
                }
            }
            return values.ToList();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
