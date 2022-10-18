using Authentication;
using DataAccessLayer;

namespace BusinessLayer;
public class AuthService : IAuthService
{
    private readonly IUnitofWork _unitOfWork;
    public AuthService(IUnitofWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task SaveRefreshToken(RefreshToken reFreshToken)
    {
        await _unitOfWork.RefreshTokens.Add(reFreshToken);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<RefreshToken> IsRefreshTokenExist(TokenRequestDto tokenRequest)
    {
        return await _unitOfWork.RefreshTokens.GetByRefreshToken(tokenRequest.RefreashToken);
    }

    public async Task MakeRefreshTokenAsUed(RefreshToken refreshToken)
    {
        var token= await _unitOfWork.RefreshTokens.GetByRefreshToken(refreshToken.Token);
        token.IsUsed= refreshToken.IsUsed;
        await _unitOfWork.CompleteAsync();
    }
}
