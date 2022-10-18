using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected ShoppingDBContext _dbContext;

    internal DbSet<T> dbSet;

    protected readonly ILogger _logger;

    public GenericRepository(
        ShoppingDBContext dbContext,
        ILogger logger
        )
    {
        _dbContext = dbContext;
        _logger = logger;
        dbSet = dbContext.Set<T>();
    }
    public virtual async Task<bool> Add(T entity)
    {
        await dbSet.AddAsync(entity);
        return true;
    }

    public virtual async Task<IEnumerable<T>> All()
    {
       return await dbSet.ToListAsync();
    }

    public virtual async Task<bool> Delete(Guid id, string userId)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<T> GetById(Guid id)
    {
        return await dbSet.FindAsync(id);
    }

    public virtual async Task Upsert(T entity)
    {
       dbSet.Update(entity);
    }

    public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                           string includeProperties = "", int start = 0, int length = 0)
    {
        IQueryable<T> query = dbSet;
        if (predicate != null)
            query = query.Where(predicate);

        foreach (var includeProperty in includeProperties.Split
            (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            query = query.Include(includeProperty);

        if (orderBy != null)
            query = orderBy(query);

        if (length != 0)
            query = query.Skip(start).Take(length);

        return await query.ToListAsync();
    }
}

