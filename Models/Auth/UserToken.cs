namespace cuzzle_api.Models.Auth;

public class UserToken
{
    public Guid Id { get; set; }
    public byte[] RefreshToken { get; set; } = new byte[32];
    public DateTime RefreshTokenExpireDate { get; set; }
}
