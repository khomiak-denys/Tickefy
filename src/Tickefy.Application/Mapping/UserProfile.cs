using AutoMapper;
using Tickefy.Application.Users.Common;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Domain.Users.User, UserDetailsResult>();
            CreateMap<Domain.Users.User, UserResult>();
        }
    }
}
