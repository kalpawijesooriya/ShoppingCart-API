using Authentication;
using AutoMapper;
using DataAccessLayer;

namespace BusinessLayer.Profiles;

    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<UserRegistrationRequestDto, User>().ReverseMap();
            CreateMap<User, UserDetailModel>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ReverseMap();

         CreateMap<User, UpdateUserModel>().ReverseMap();
    }
    }

