using AutoMapper;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Content;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Priority;
using Tickefy.Domain.Common.Status;
using Tickefy.Domain.Common.UserRole;

namespace Tickefy.Application.Mapping
{
    public class EnumProfile : Profile
    {
        public EnumProfile()
        {
            CreateMap<Category, string>().ConvertUsing(src => src.ToString());
            CreateMap<ContentType, string>().ConvertUsing(src => src.ToString());
            CreateMap<EventType, string>().ConvertUsing(src => src.ToString());
            CreateMap<Priority, string>().ConvertUsing(src => src.ToString());
            CreateMap<Status, string>().ConvertUsing(src => src.ToString());
            CreateMap<UserRoles, string>().ConvertUsing(src => src.ToString());
        }
    }
}
