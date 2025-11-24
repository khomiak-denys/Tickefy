using AutoMapper;
using Tickefy.Domain.Primitives;
namespace Tickefy.Application.Mapping
{
    public class StronglyTypedIdProfile : Profile
    {
        public StronglyTypedIdProfile()
        {
            CreateMap<ActivityLogId, Guid>().ConvertUsing(src => src.Value);
            CreateMap<AttachmentId, Guid>().ConvertUsing(src => src.Value);
            CreateMap<CommentId, Guid>().ConvertUsing(src => src.Value);
            CreateMap<UserId, Guid>().ConvertUsing(src => src.Value);
            CreateMap<TeamId, Guid>().ConvertUsing(src => src.Value);
            CreateMap<TicketId, Guid>().ConvertUsing(src => src.Value);
        }
    }
}
