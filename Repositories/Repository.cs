using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Repositories.IRepositories;
using System.Linq.Expressions;

namespace Movies.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository()
        {
            _context = new ApplicationDbContext();
            _dbSet = _context.Set<T>();
        }
        public async Task<bool> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
                
            }
        }

        public bool Delete(T entity)
        {
            try
            {
                _context.Remove(entity);
                return true;
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public T? Get(Expression<Func<T, bool>>[]? exception = null, Expression<Func<T, object>>[]? includes = null, bool Tracked = true)
        {
           return GetAll(exception,includes,Tracked).FirstOrDefault();
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>>[]? exception = null, Expression<Func<T, object>>[]? includes = null, bool Tracked = true)
        {
            IQueryable<T> entities = _dbSet;
            if (exception is not null)
            {
                foreach(var ex in exception)
                {
                    entities = entities.Where(ex);
                }
            
            }
            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }
            if (!Tracked)
            {
                entities = entities.AsNoTracking();
            }
            return entities;
        }

        public bool Update(T entity)
        {
            try
            {
                _context.Update(entity);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            } 
        }
    }
}
