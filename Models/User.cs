public class UserToken 
{
    public Guid Id { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpireDate { get; set; }
}
