using System.Security.Claims;
using cuzzle_api.Models.Auth;

namespace cuzzle_api.Services.TokenService;

public interface ITokenService
{
    public string GenerateAccessToken(Guid id);

    public RefreshToken GeneretateRefreshToken(Guid id);

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
