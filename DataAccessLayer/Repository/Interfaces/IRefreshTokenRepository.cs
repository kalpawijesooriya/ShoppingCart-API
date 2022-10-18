namespace DataAccessLayer;

public interface IRefreshTokenRepository:IGenericRepository<RefreshToken>
{
    Task<RefreshToken> GetByRefreshToken(string refreshtoken);
}
