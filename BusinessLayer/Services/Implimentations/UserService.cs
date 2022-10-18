using Authentication;
using AutoMapper;
using DataAccessLayer;
using DataAccessLayer.Migrations;
using System.Linq.Expressions;

namespace BusinessLayer;

public class UserService : IUserService
{
    private IUnitofWork _unitofWork;

    private IMapper _mapper;

    public UserService
        (IUnitofWork unitofWork,
        IMapper mapper)
    {
        _unitofWork = unitofWork;
        _mapper = mapper;
    }

    public async Task<Response> SaveUser(UserRegistrationRequestDto registrationRequestDto)
    {
        try
        {
            var user = _mapper.Map<User>(registrationRequestDto);
            await _unitofWork.Users.Add(user);
            await _unitofWork.CompleteAsync();
            return new Response { IsSuccess = true, Data = _mapper.Map<User, UserDetailModel>(user) };
        }
        catch (Exception ex)
        {
            return new Response { IsSuccess = false, Error = ex.Message };
        }
    }

    public async Task<Response> GetUsers()
    { 
        try
        {
            var users = await _unitofWork.Users.All();
            return new Response { IsSuccess = true, Data = _mapper.Map<IEnumerable<User>, List<UserDetailModel>>(users) };
        }
        catch (Exception ex)
        {
            return new Response { IsSuccess = false, Error = ex.Message };
        }
    }

    public async Task<Response> GetUser(Guid userid)
    {
        try
        {
            var user = await _unitofWork.Users.GetById(userid);
            return new Response { IsSuccess = true, Data = _mapper.Map<User, UserDetailModel>(user) };
        }
        catch (Exception ex)
        {
            return new Response { IsSuccess = false, Error = ex.Message };
        }
    }

    public async Task<Response> UpdateUserProfile(UpdateUserModel model)
    {
        try
        {
            Expression<Func<User, bool>> filter = user =>
                  user.Status == 1 && user.Id == model.UserId;

            var user = (await _unitofWork.Users.GetAsync(filter)).FirstOrDefault();
            if (user==null) return new Response {IsSuccess=false,Error="No user found!" };
    
            var updatedUser=_mapper.Map(model, user);
            await _unitofWork.CompleteAsync();
            
            return new Response { IsSuccess = true, Data= _mapper.Map<User, UserDetailModel>(updatedUser) };
        }
        catch (Exception ex)
        {
            return new Response { IsSuccess = false, Error = ex.Message };
        }
    }

    public async Task<Response> GetUserByIdentityId(Guid identityId)
    {
        try
        {
            Expression<Func<User, bool>> filter = user =>
                user.Status == 1 && user.IdentityId == identityId;

            var user = (await _unitofWork.Users.GetAsync(filter)).FirstOrDefault();
            if (user == null) return new Response { IsSuccess = false, Error = "No user found!" };

            return new Response { IsSuccess = true, Data = _mapper.Map<User, UserDetailModel>(user) };
        }
        catch (Exception ex)
        {
            return new Response { IsSuccess = false, Error = ex.Message };
        }
    }
}
