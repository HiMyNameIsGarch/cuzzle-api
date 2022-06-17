using cuzzle_api.Services.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cuzzle_api.Models.Auth;
using System.Security.Claims;
using cuzzle_api.Services.AuthService;

namespace cuzzle_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController: ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly ITokenService _tokenService;
    private readonly IAuthService _auth;

    public TokenController(ILogger<TokenController> logger, ITokenService tokenService, IAuthService auth)
    {
        _auth = auth;
        _logger = logger;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] AuthenticationResponse model)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(model.Token);

        string userId = principal.FindFirstValue(ClaimTypes.Sid);
        if(!Guid.TryParse(userId, out Guid goodUserId))
            return Unauthorized("Invalid Sid claim!");

        // get user from db
        var user = _auth.GetUserToken(goodUserId);
        // Check token
        if(!_auth.CheckIfTokensMatch(model.RefreshToken, user) || user.Id == Guid.Empty)
            return Unauthorized("Invalid Refresh Token!");

        // generate new tokens
        var token = _tokenService.GenerateAccessToken(goodUserId);
        var newRefreshToken = _tokenService.GeneretateRefreshToken(goodUserId);
        if(string.IsNullOrEmpty(newRefreshToken.Token)) return BadRequest("We could not generate refresh token!");

        var response = new AuthenticationResponse()
        {
            Token = token,
            RefreshToken = newRefreshToken.Token
        };
        return Ok(response);
    }
}
