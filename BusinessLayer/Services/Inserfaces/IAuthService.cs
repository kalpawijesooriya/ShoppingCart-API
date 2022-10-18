using Authentication;
using DataAccessLayer;

namespace BusinessLayer;

public interface IAuthService
{
    Task SaveRefreshToken(RefreshToken registrationRequestDto);
    Task<RefreshToken> IsRefreshTokenExist(TokenRequestDto tokenRequest);
    Task MakeRefreshTokenAsUed(RefreshToken refreshToken);
}
