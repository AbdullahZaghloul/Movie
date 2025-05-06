using System.Linq.Expressions;

namespace Movies.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<bool> AddAsync(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        IQueryable<T> GetAll(Expression<Func<T, bool>>[]? exception = null,
            Expression<Func<T, object>>[]? includes = null, bool Tracked = true);

        T? Get(Expression<Func<T, bool>>[]? exception = null,
            Expression<Func<T, object>>[]? includes = null, bool Tracked = true);

        Task<bool> CommitAsync();
    }
}
