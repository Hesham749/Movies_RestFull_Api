namespace MoviesApi.Interfaces
{
    public interface IUnitOfWorks:IDisposable
    {
        IBaseRepository<Genre> Genres { get; }
        IBaseRepository<Movie> Movies { get; }
        void Save();
    }
}
