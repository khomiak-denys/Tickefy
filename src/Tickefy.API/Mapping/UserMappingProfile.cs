using AutoMapper;
using Tickefy.API.User.Responses;
using Tickefy.Application.Users.Common;

namespace Tickefy.API.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserDetailsResult, UserResponse>();
            CreateMap<UserResult, MinimalUserResponse>();
        }
    }
}
