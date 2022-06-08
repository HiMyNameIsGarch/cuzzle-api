namespace cuzzle_api.Services.AuthService;

public interface IAuthService {

    public bool UserExists(UserLogin user);

    public bool Register(UserLogin register);

    public Guid Authenticate(UserLogin user);

    public bool CheckIfTokensMatch(string userToken, UserToken dbToken);

    public UserToken GetUserToken(Guid id);
}
