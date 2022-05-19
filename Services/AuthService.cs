using cuzzle_api.Models;
using Npgsql;

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
