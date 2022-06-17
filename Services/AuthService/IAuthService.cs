using cuzzle_api.Models.Auth;

namespace cuzzle_api.Services.AuthService;

public interface IAuthService {

    public bool UserExists(string email);

    public bool Register(UserRegister register);

    public Guid Authenticate(UserLogin login);

    public bool CheckIfTokensMatch(string userToken, UserToken dbToken);

    public UserToken GetUserToken(Guid id);
}
