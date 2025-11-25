using AutoMapper;
using Tickefy.Application.User.Common;
using Tickefy.Domain.Common.UserRole;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<UserId, Guid>().ConvertUsing(src => src.Value);
            CreateMap<TeamId, Guid>().ConvertUsing(src => src.Value);

            CreateMap<UserRoles, string>().ConvertUsing(src => src.ToString());

            CreateMap<Domain.User.User, UserResult>();
        }
    }
}
