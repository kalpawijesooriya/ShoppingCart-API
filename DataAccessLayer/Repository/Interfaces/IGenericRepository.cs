using System.Linq.Expressions;

namespace DataAccessLayer
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> All();

        Task<T> GetById(Guid id);

        Task<bool> Add(T entity);

        Task Upsert(T entity);

        Task<bool> Delete(Guid id, string userId);

        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                    string includeProperties = "", int start = 0, int length = 0);
    }
}
