using Authentication;
using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi;
public class AuthHelper : IAuthHelper
{
    private readonly JwtConfig _jwtConfig;

    private readonly IAuthService _authService;

    private readonly TokenValidationParameters _tokenValidationParameters;

    private readonly UserManager<IdentityUser> _userManager;
    public AuthHelper
    (
     IOptionsMonitor<JwtConfig> optionsMonitor,
     IAuthService authService,
     TokenValidationParameters tokenValidationParameters,
     UserManager<IdentityUser> userManager
    )
    {
        _jwtConfig = optionsMonitor.CurrentValue;
        _authService = authService;
        _tokenValidationParameters = tokenValidationParameters;
        _userManager = userManager;

    }
    public async Task<TokenData> GenerateJWTToken(IdentityUser user)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.Secrect);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            }),
            Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = jwtHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtHandler.WriteToken(token);
        var reFreshToken = new RefreshToken
        {
            AddedDate = DateTime.UtcNow,
            Token = $"{RandomStringGenerator(25)}_{Guid.NewGuid()}",
            UserId = user.Id,
            IsUsed = false,
            IsRevorked = false,
            ExpiryDate = DateTime.UtcNow.AddMonths(6),
            JwtId = token.Id
        };
        await _authService.SaveRefreshToken(reFreshToken);
        return new TokenData { JwtToken = jwtToken, RefreshToken = reFreshToken.Token };
    }

    public async Task<AuthResult?> GenerateTokenByRefreshToken(TokenRequestDto tokenRequest)
    {
        var tokenHanddler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHanddler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

            if (!CheckTokenAlgo(validatedToken))
                return null;

            var expireResult = CheckExpireDate(principal);
            if (!expireResult.Success)
                return expireResult;

            var refreshToken = await _authService.IsRefreshTokenExist(tokenRequest);
            var tokenStatus = CheckRefreshToken(refreshToken);
            if (!tokenStatus.Success)
                return tokenStatus;

            var jtiStatus = CheckJti(principal, refreshToken);
            if (!jtiStatus.Success)
                return jtiStatus;

            refreshToken.IsUsed = true;
            await _authService.MakeRefreshTokenAsUed(refreshToken);
            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            if (user == null)
            {
                return new AuthResult() { Success = false, Errors = new List<string>() { "Error processing request" } };
            }

            var tokens = await GenerateJWTToken(user);
            return new AuthResult()
            {
                Success = true,
                Token = tokens.JwtToken,
                RefreshToken = tokens.RefreshToken
            };
        }
        catch (Exception ex)
        {
            return null; //ToDo
        }
    }

    private string RandomStringGenerator(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private DateTime UnixTimeStampToDateTime(long unixDate)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixDate).ToUniversalTime();
        return dateTime;
    }

    private bool CheckTokenAlgo(SecurityToken validatedToken)
    {
        if (validatedToken is JwtSecurityToken jwtSecurityToken)
        {
            var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

            if (!result)
                return false;
        }
        return true;
    }

    private AuthResult CheckExpireDate(ClaimsPrincipal principal)
    {
        var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

        var expDate = UnixTimeStampToDateTime(utcExpiryDate);

        if (expDate > DateTime.UtcNow)
        {
            return new AuthResult() { Success = false, Errors = new List<string>() { "Jwt token has not expired" } };
        }
        return new AuthResult() { Success = true };
    }

    private AuthResult CheckJti(ClaimsPrincipal principal, RefreshToken refreshToken)
    {
        var jti = principal?.Claims?.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

        if (refreshToken.JwtId != jti)
        {
            return new AuthResult() { Success = false, Errors = new List<string>() { "Refresh token refreance doses not match with the jwt token" } };
        }
        return new AuthResult() { Success = true };
    }

    private AuthResult CheckRefreshToken(RefreshToken refreshToken)
    {
        if (refreshToken == null)
        {
            return new AuthResult()
            {
                Success = false,
                Errors = new List<string>()
                    {
                      "Invalied refresh token"
                    }
            };
        }

        if (refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return new AuthResult()
            {
                Success = false,
                Errors = new List<string>()
                    {
                      "Refresh token has expired."
                    }
            };
        }
        if (refreshToken.IsUsed)
        {
            return new AuthResult()
            {
                Success = false,
                Errors = new List<string>()
                    {
                      "Refresh token has used."
                    }
            };
        }
        if (refreshToken.IsRevorked)
        {
            return new AuthResult()
            {
                Success = false,
                Errors = new List<string>()
                    {
                      "Refresh token has Revorked."
                    }
            };
        }
        else
        {
            return new AuthResult()
            {
                Success = true
            };
        }
    }
}
