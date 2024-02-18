using Microsoft.EntityFrameworkCore;
using Moiniorell.Application.Abstractions.Repositories.Generic;
using Moiniorell.Persistence.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Persistence.Implementations.Repositories.Generic
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        private readonly AppDbContext _db;

        public Repository(AppDbContext db)
        {
            _db = db;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>>? expression = null, params string[] includes)
        {
            var query = _db.Set<T>().AsQueryable();

            if (expression != null)
            {
                query = query.Where(expression);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> expression, params string[] includes)
        {
            var query = _db.Set<T>().AsQueryable();

            query = query.Where(expression);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _db.Set<T>().AnyAsync(expression);
        }

        public async Task CreateAsync(T entity)
        {
            await _db.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _db.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _db.Set<T>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }

}
