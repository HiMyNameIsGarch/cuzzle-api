using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using cuzzle_api.Models.Helpers;
using cuzzle_api.Models.Auth;

namespace cuzzle_api.Services.TokenService;

public class TokenService: ITokenService
{
    private const string SECURITYALGO = SecurityAlgorithms.HmacSha256;

    private readonly IConfiguration _config;

    private readonly IDbService _db;

    public TokenService(IConfiguration config, IDbService db)
    {
        _config = config;
        _db = db;
    }

    private SymmetricSecurityKey getIssuerSigningKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = getIssuerSigningKey(),
            ValidateLifetime = false,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SECURITYALGO, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    public string GenerateAccessToken(Guid id)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, id.ToString())
        };

        var key = getIssuerSigningKey();

        var cred = new SigningCredentials(key, SECURITYALGO);

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
            Token = TokenHelper.GetRandomToken(),
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

        byte[] hashedToken = TokenHelper.HashToken(token.Token);
        cmd.Parameters.AddWithValue("refresh_token", hashedToken);
        cmd.Parameters.AddWithValue("refresh_token_expire_date", token.Expires);

        return _db.ExecuteQuery(cmd);
    }
}
