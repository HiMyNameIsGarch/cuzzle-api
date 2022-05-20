using cuzzle_api.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Npgsql;
using System.Security.Cryptography;

public class AuthService
{
    private readonly CuzzleEntity _db;

    public AuthService()
    {
        _db = new CuzzleEntity();
    }

    private byte[] GenerateSalt()
    {
        byte[] salt = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    private byte[] GeneratePasswordHash(string password, byte[] salt)
    {
        byte[] passwordHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 50000,
            numBytesRequested: 256);
        return passwordHash;
    }

    public bool UserExists(UserLogin user)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "SELECT id FROM account WHERE email = @email;";
        cmd.Parameters.AddWithValue("email", user.Email);

        Guid id = _db.GetScalar<Guid>(cmd);
        return id != Guid.Empty;
    }

    public bool Register(UserLogin register)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "INSERT INTO account(username, email, password_hash, password_salt) VALUES(@username, @email, @password_hash, @password_salt) RETURNING id;";
        cmd.Parameters.AddWithValue("username", register.UserName);
        cmd.Parameters.AddWithValue("email", register.Email);
        // Generate salt
        byte[] salt = GenerateSalt();
        // Generate password with the salt
        byte[] password = GeneratePasswordHash(register.Password, salt);
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
        if(MatchPasswords(user.Password, result.PasswordHash, result.PasswordSalt))
            return result.Id;

        return Guid.Empty;
    }

    private bool MatchPasswords(string password, byte[] passwordDb, byte[] saltDb)
    {
        byte[] pass = GeneratePasswordHash(password, saltDb);

        return pass.SequenceEqual(passwordDb);
    }

    public UserToken GetUserToken(Guid id)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "SELECT id, refresh_token, refresh_token_expire_date FROM account WHERE id = @id;";
        cmd.Parameters.AddWithValue("id", id);

        UserToken user = _db.GetObject<UserToken>(cmd);
        user.RefreshToken = user.RefreshToken.Trim();
        return user;
    }
}
