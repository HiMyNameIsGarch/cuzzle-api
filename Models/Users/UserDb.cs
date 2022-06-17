namespace cuzzle_api.Models;

public class UserDb
{
    public Guid Id { get; set; }
    public byte[] PasswordHash { get; set; } = new byte[256];
    public byte[] PasswordSalt { get; set; } = new byte[64];

    public UserDb() {}
}
