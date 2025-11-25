using AutoMapper;
using Tickefy.API.User.Responses;
using Tickefy.Application.User.Common;

namespace Tickefy.API.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile() 
        {
            CreateMap<UserResult, UserResponse>()
                .ForMember(
                    dest => dest.Role,
                    opt => opt.MapFrom(src => src.Role.ToString()));
        }
    }
}
