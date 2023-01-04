using System.Security.Claims;

namespace cuzzle_api.Services.UserService;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetId()
    {
        string tmp = string.Empty;
        if(_httpContextAccessor.HttpContext is not null)
        {
            tmp = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
        }
        Guid result = new Guid(tmp);
        return result;
    }
}
