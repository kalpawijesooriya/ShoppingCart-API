namespace DataAccessLayer;

public interface IUnitofWork
{
    IUsersRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    Task CompleteAsync();
}
