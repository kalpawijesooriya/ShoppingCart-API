using Authentication;

namespace BusinessLayer;

public interface IUserService
{
   Task<Response> SaveUser(UserRegistrationRequestDto registrationRequestDto);

   Task<Response> GetUsers();

   Task<Response> GetUser(Guid userid);

    Task<Response> UpdateUserProfile(UpdateUserModel model);

    Task<Response> GetUserByIdentityId(Guid identityId);
}
