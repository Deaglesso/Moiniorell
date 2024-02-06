using System.Linq.Expressions;

namespace Moiniorell.Application.Abstractions.Repositories.Generic
{
    public interface IRepository<T> where T : class,new()
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>>? expression = null, params string[] includes);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> expression, params string[] includes);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task CreateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveChangesAsync();
    }
}
