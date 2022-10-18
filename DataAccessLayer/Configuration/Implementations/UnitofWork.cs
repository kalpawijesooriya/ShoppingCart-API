using Microsoft.Extensions.Logging;

namespace DataAccessLayer
{
    public class UnitofWork : IUnitofWork,IDisposable
    {
        private ShoppingDBContext _dbContext;

        private readonly ILogger _logger;

        public IUsersRepository Users { get; private set; }

        public IRefreshTokenRepository RefreshTokens  { get; private set; }

        public UnitofWork(ShoppingDBContext dbContext,ILoggerFactory loggerFactory)
        {
            _dbContext = dbContext;
            Users = new UsersRepository(dbContext, _logger);
            RefreshTokens=new RefreshTokenRepository(dbContext, _logger);
        }

        public async Task CompleteAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        { 
          _dbContext.Dispose();
        }
    }
}
