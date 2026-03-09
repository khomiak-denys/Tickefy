using AutoMapper;
using Tickefy.API.Auth.Responses;
using Tickefy.Application.Auth.Common;

namespace Tickefy.API.Mapping
{
    public class LoginMappingProfile : Profile
    {
        public LoginMappingProfile() {
            CreateMap<LoginResult, LoginResponse>();
        }
    }
}
