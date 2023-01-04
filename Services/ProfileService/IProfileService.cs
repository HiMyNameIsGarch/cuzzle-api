using cuzzle_api.Models.Profile;

namespace cuzzle_api.Services.ProfileService;

public interface IProfileService {

    public UserProfile GetProfile(Guid id);

}
