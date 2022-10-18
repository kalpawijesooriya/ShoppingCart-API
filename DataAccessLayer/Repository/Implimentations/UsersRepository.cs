
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer;

public class UsersRepository : GenericRepository<User>, IUsersRepository
{
    public UsersRepository(
        ShoppingDBContext dbContext,
        ILogger logger) : base(dbContext, logger)
    {

    }

    public override async Task<IEnumerable<User>> All()
    {
        try
        {
            return await dbSet.Where(x => x.Status == 1)
                               .AsNoTracking()
                               .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ typeof(UsersRepository)} All methord generated an error");
            return Enumerable.Empty<User>();
        }
    }
}

