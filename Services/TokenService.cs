using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using cuzzle_api.Models;
using cuzzle_api.Services;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

public class TokenService: ITokenService
{
    private const string security_algo = SecurityAlgorithms.HmacSha256;

    private readonly IConfiguration _config;

    private readonly CuzzleEntity _db;

    public TokenService(IConfiguration config)
    {
        _config = config;
        _db = new CuzzleEntity();
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
            ValidateLifetime = false,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(security_algo, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    public string GenerateAccessToken(Guid id)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _config.GetSection("Jwt:Key").Value));

        var cred = new SigningCredentials(key, security_algo);

        var token = new JwtSecurityToken(
                issuer: _config.GetSection("Jwt:Issuer").Value,
                audience: _config.GetSection("Jwt:Audience").Value,
                claims: claims, 
                expires: DateTime.Now.AddMinutes(_config.GetValue<double>("Jwt:AccessExpireTime")),
                signingCredentials: cred);

        string jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public RefreshToken GeneretateRefreshToken(Guid id)
    {
        // gen refresh token
        var token = new RefreshToken
        {
            Token = GetRandomToken(),
            Expires = DateTime.Now.AddDays(_config.GetValue<double>("Jwt:RefreshExpireTime")),
            Created = DateTime.Now
        };
        // return it
        if(!AddTokenToDb(token, id)) return new RefreshToken();
        return token;
    }

    private bool AddTokenToDb(RefreshToken token, Guid id)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "UPDATE account SET refresh_token = @refresh_token, refresh_token_expire_date = @refresh_token_expire_date WHERE id = @id::UUID;";
        cmd.Parameters.AddWithValue("id", id);

        byte[] hashedToken = HashToken(token.Token);
        cmd.Parameters.AddWithValue("refresh_token", hashedToken);
        cmd.Parameters.AddWithValue("refresh_token_expire_date", token.Expires);

        return _db.ExecuteQuery(cmd);
    }

    // TODO: Move this function into a proper class
    private byte[] HashToken(string token)
    {
        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
        byte[] hash = new byte[32];
        using(SHA256 sha = SHA256.Create())
        {
            hash = sha.ComputeHash(tokenBytes);
        }
        return hash;
    }

    private string GetRandomToken()
    {
        string token = string.Empty;
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            token = Convert.ToBase64String(randomNumber);
        }
        return token;
    }

}
