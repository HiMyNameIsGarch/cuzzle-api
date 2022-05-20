using cuzzle_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cuzzle_api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController: ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    private readonly AuthService auth;
    private readonly TokenService _token;
    private readonly IConfiguration _config;

    public AuthController(ILogger<AuthController> logger, IConfiguration config)
    {
        auth = new AuthService();
        _token = new TokenService(config);
        _logger = logger;
        _config = config;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register([FromBody] UserLogin userLogin)
    {
        if(auth.UserExists(userLogin)) return BadRequest("User already exists! Try loggin in instead!");

        bool registered = auth.Register(userLogin);
        if(registered) return Ok();
        return BadRequest("We could not register the user.");
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin user)
    {
        if(!auth.UserExists(user)) return BadRequest("User does not exists! Try to register first!");

        Guid id = auth.Authenticate(user);
        if(id == Guid.Empty) return BadRequest("Incorrect password!");

        var token = _token.GenerateAccessToken(id);
        var refreshToken = _token.GeneretateRefreshToken(id);
        if(string.IsNullOrEmpty(refreshToken.Token)) return BadRequest("We could not generate refresh token!");

        var tokens = new AuthenticatedResponse()
        {
            Token = token,
            RefreshToken = refreshToken.Token
        };
        return Ok(tokens);
    }
}
