
using System.Linq.Expressions;

namespace MoviesApi.Repositories
{
    public class MovieRepository : BaseRepository<Movie>
    {
        public MovieRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Movie>> GetAllAsync(Expression<Func<Movie, bool>> criteria = null)
        {
            IQueryable<Movie> query = _dbSet;
            if (criteria != null)
                query = query.Where(criteria);
            query = query.Include(m => m.Genre);
            query = query.OrderByDescending(m => m.Rate);
            query = query.AsNoTracking();
            return await query.ToListAsync();
        }

        public override async Task<Movie> GetByIdAsync(int id)
            => await _dbSet.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);
    }
}
