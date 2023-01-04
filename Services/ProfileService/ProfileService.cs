using cuzzle_api.Models.Profile;
using Npgsql;

namespace cuzzle_api.Services.ProfileService;

public class ProfileService : IProfileService
{
    private readonly IDbService _db;

    public ProfileService(IDbService db)
    {
        _db = db;
    }

    public UserProfile GetProfile(Guid id)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "SELECT username, email, days_entered FROM account WHERE id = @id::UUID;";
        cmd.Parameters.AddWithValue("id", id);

        UserProfile profile = _db.GetObject<UserProfile>(cmd);
        return profile;
    }
}
