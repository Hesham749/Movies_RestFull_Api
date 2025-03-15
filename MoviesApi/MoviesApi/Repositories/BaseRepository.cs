using System.Collections.Immutable;
using System.Linq.Expressions;


namespace MoviesApi.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T>
        where T : class


    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> criteria = null)
        {
            criteria ??= T => true;
            return await _dbSet.AnyAsync(criteria);
        }

        public void Delete(T entity) => _dbSet.Remove(entity);

        public abstract Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> criteria = null);


        public abstract Task<T> GetByIdAsync(int id);


        public void Update(T entity) => _dbSet.Update(entity);
    }
}
