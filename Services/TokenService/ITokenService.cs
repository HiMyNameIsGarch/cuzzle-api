using System.Security.Claims;

namespace cuzzle_api.Services.TokenService;

public interface ITokenService
{
    public string GenerateAccessToken(Guid id);

    public RefreshToken GeneretateRefreshToken(Guid id);

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
