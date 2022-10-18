using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(
        ShoppingDBContext dbContext,
        ILogger logger) : base(dbContext, logger)
    {

    }

    public override async Task<IEnumerable<RefreshToken>> All()
    {
        try
        {
            return await dbSet.Where(x => x.Status == 1)
                               .AsNoTracking()
                               .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{typeof(UsersRepository)} All methord generated an error");
            return Enumerable.Empty<RefreshToken>();
        }
    }

    public async Task<RefreshToken> GetByRefreshToken(string refreshtoken)
    {
        try
        {
            return await dbSet.Where(x => x.Token.ToLower() == refreshtoken.ToLower())
                               .AsNoTracking()
                               .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{typeof(UsersRepository)} All methord generated an error");
            return null;
        }
    }
}
