namespace cuzzle_api.Models;

public class AuthenticatedResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
