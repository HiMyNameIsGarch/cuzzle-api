using cuzzle_api.Services.ProfileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cuzzle_api.Controllers;

[ApiController]
[Authorize]
[Route("api/profile")]
public class ProfileController: ControllerBase
{
    private readonly ILogger<ProfileController> _logger;
    private readonly IUserService _userService;
    private readonly IProfileService _ps;

    public ProfileController(ILogger<ProfileController> logger, IUserService user, IProfileService profile)
    {
        _logger = logger;
        _userService = user;
        _ps = profile;
    }

    [HttpGet]
    public IActionResult Get()
    {
        Guid id = _userService.GetId();
        if(id == Guid.Empty)
        {
            return BadRequest("Invalid Id from JWT token!");
        }

        var profile = _ps.GetProfile(id);
        return Ok(profile);
    }
}

