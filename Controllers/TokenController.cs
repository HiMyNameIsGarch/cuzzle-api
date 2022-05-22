using cuzzle_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cuzzle_api.Models;
using System.Security.Claims;

namespace cuzzle_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController: ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly ITokenService _tokenService;
    private readonly AuthService auth;

    public TokenController(ILogger<TokenController> logger, ITokenService tokenService)
    {
        auth = new AuthService();
        _logger = logger;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] AuthenticatedResponse model)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(model.Token);

        string userId = principal.FindFirstValue(ClaimTypes.Sid);
        if(!Guid.TryParse(userId, out Guid goodUserId)) 
            return Unauthorized("Invalid Sid claim!");

        // get user from db
        var user = auth.GetUserToken(goodUserId);
        // Check token
        if(!auth.CheckIfTokensMatch(model.RefreshToken, user) || user.Id == Guid.Empty)
            return Unauthorized("Invalid Refresh Token!");

        // generate new tokens
        var token = _tokenService.GenerateAccessToken(goodUserId);
        var newRefreshToken = _tokenService.GeneretateRefreshToken(goodUserId);
        if(string.IsNullOrEmpty(newRefreshToken.Token)) return BadRequest("We could not generate refresh token!");

        return Ok(new AuthenticatedResponse(){ Token = token, RefreshToken = newRefreshToken.Token });
    }
}
