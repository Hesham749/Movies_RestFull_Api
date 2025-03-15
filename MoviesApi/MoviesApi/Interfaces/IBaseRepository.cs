using System.Data;
using System.Linq.Expressions;

namespace MoviesApi.Interfaces
{
    public interface IBaseRepository<T>
        where T : class
    {
        Task<T> GetByIdAsync(int id)  ;
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> criteria = null);
        Task AddAsync(T entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> criteria = null);
        void Update(T entity);
        void Delete(T entity);
    }
}
