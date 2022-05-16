using System.Security.Claims;

namespace cuzzle_api.Services.UserService;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetId()
    {
        string result = string.Empty;
        if(_httpContextAccessor.HttpContext is not null)
        {
            result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
        }
        return result;
    }
}
