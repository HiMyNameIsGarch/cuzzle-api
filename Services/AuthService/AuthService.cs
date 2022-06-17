using cuzzle_api.Models;
using cuzzle_api.Models.Helpers;
using cuzzle_api.Models.Auth;
using Npgsql;

namespace cuzzle_api.Services.AuthService;

public class AuthService: IAuthService
{
    private readonly IDbService _db;

    public AuthService(IDbService db)
    {
        _db = db;
    }

    public bool UserExists(string email)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "SELECT id FROM account WHERE email = @email;";
        cmd.Parameters.AddWithValue("email", email);

        Guid id = _db.GetScalar<Guid>(cmd);
        return id != Guid.Empty;
    }

    public bool Register(UserRegister register)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "INSERT INTO account(username, email, password_hash, password_salt) VALUES(@username, @email, @password_hash, @password_salt) RETURNING id;";
        cmd.Parameters.AddWithValue("username", register.UserName);
        cmd.Parameters.AddWithValue("email", register.Email);
        // Generate salt
        byte[] salt = SecurityHelper.GenerateSalt();
        // Generate password with the salt
        byte[] password = SecurityHelper.GeneratePasswordHash(register.Password, salt);
        // Store both
        cmd.Parameters.AddWithValue("password_hash", password);
        cmd.Parameters.AddWithValue("password_salt", salt);

        // Execute query
        return _db.ExecuteQuery(cmd);
    }

    public Guid Authenticate(UserLogin user)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "SELECT id, password_hash, password_salt FROM account WHERE email = @email;";
        cmd.Parameters.AddWithValue("email", user.Email);

        var result = _db.GetObject<UserDb>(cmd);
        if(SecurityHelper.MatchPasswords(user.Password, result.PasswordHash, result.PasswordSalt))
            return result.Id;

        return Guid.Empty;
    }

    public UserToken GetUserToken(Guid id)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "SELECT id, refresh_token, refresh_token_expire_date FROM account WHERE id = @id;";
        cmd.Parameters.AddWithValue("id", id);

        UserToken user = _db.GetObject<UserToken>(cmd);
        return user;
    }

    public bool CheckIfTokensMatch(string userToken, UserToken dbToken)
    {
        if(dbToken.RefreshTokenExpireDate < DateTime.Now) return false;

        byte[] hashedToken = TokenHelper.HashToken(userToken);

        return hashedToken.SequenceEqual(dbToken.RefreshToken);
    }
}
