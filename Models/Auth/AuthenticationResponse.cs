namespace cuzzle_api.Models.Auth;

public class AuthenticationResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
