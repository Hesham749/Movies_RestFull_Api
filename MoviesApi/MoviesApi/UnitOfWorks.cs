using MoviesApi.Interfaces;
using MoviesApi.Repositories;

namespace MoviesApi
{
    public class UnitOfWorks : IUnitOfWorks
    {
        #region Fields
        private readonly ApplicationDbContext _context;
        private readonly IBaseRepository<Genre> _genreRepo;
        private readonly IBaseRepository<Movie> _movieRepo;
        #endregion
        #region Properties
        public UnitOfWorks(ApplicationDbContext context)
        {
            _context = context;
        }

        public IBaseRepository<Genre> Genres
        {
            get
            {
                if (_genreRepo == null)
                    return new GenreRepository(_context);
                return _genreRepo;
            }

        }

        public IBaseRepository<Movie> Movies
        {
            get
            {
                if (_movieRepo == null)
                    return new MovieRepository(_context);
                return _movieRepo;
            }
        }
        #endregion

        public void Dispose() => _context.Dispose();

        public void Save() => _context.SaveChanges();

    }
}
