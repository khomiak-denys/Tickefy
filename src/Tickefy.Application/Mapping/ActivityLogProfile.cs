using AutoMapper;
using Tickefy.Application.ActivityLog.Common;

namespace Tickefy.Application.Mapping
{
    public class ActivityLogProfile : Profile
    {
        public ActivityLogProfile()
        {
            CreateMap<Domain.ActivityLog.ActivityLog, LogResult>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.TicketId,
                    opt => opt.MapFrom(src => src.TicketId.Value))
                .ForMember(dest => dest.UserId,
                    opt => opt.MapFrom(src => src.UserId.Value))
                .ForMember(dest => dest.EventType,
                    opt => opt.MapFrom(src => src.EventType.ToString()))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.Description));
        }
    }
}
