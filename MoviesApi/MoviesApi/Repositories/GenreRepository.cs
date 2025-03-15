
using System.Linq.Expressions;

namespace MoviesApi.Repositories
{
    public class GenreRepository : BaseRepository<Genre>
    {
        public GenreRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Genre>> GetAllAsync(Expression<Func<Genre, bool>> criteria = null)
        {
            IQueryable<Genre> query = _dbSet;
            if (criteria != null)
                query = query.Where(criteria);
            query = query.OrderBy(g => g.Name);
            return await query.ToListAsync();
        }



        public override async Task<Genre> GetByIdAsync(int id)
        {
            return await _dbSet.SingleOrDefaultAsync(x => x.Id == id);
        }


    }
}
