using Authentication;
using BusinessLayer;
using Microsoft.AspNetCore.Identity;

namespace WebApi
{
    public interface IAuthHelper
    {
       Task<TokenData> GenerateJWTToken(IdentityUser user);
       Task<AuthResult> GenerateTokenByRefreshToken(TokenRequestDto tokenRequest);
    }
}
