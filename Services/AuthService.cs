using cuzzle_api.Models;
using Npgsql;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

public class AuthService
{
    private readonly CuzzleEntity _db;

    public AuthService()
    {
        _db = new CuzzleEntity();
    }

    public bool Register(UserLogin register)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "INSERT INTO account(username, email, password_hash) VALUES(@username, @email, @password_hash) RETURNING id;";
        cmd.Parameters.AddWithValue("username", register.UserName);
        cmd.Parameters.AddWithValue("email", register.Email);
        cmd.Parameters.AddWithValue("password_hash", register.Password);
        bool created = _db.ExecuteQuery(cmd);

        return created;
    }

    public Guid Authenticate(UserLogin user)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "SELECT id FROM account WHERE email = @email AND password_hash = @password_hash;";
        cmd.Parameters.AddWithValue("email", user.Email);
        cmd.Parameters.AddWithValue("password_hash", user.Password);

        var result = _db.GetScalar(cmd).ToString();
        Guid.TryParse(result, out Guid id);

        return id;
    }

    public string GenerateToken(Guid id, IConfiguration config)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    config.GetSection("Jwt:Key").Value));

        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
                issuer: config.GetSection("Jwt:Issuer").Value,
                audience: config.GetSection("Jwt:Audience").Value,
                claims: claims, 
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: cred);

        string jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
