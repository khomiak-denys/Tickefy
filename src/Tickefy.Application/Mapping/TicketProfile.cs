using AutoMapper;
using Tickefy.Application.Ticket.Common;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Priority;
using Tickefy.Domain.Common.Status;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Common.Mapping
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<TicketId, Guid>().ConvertUsing(src => src.Value);
            CreateMap<UserId, Guid>().ConvertUsing(src => src.Value);
            CreateMap<TeamId, Guid>().ConvertUsing(src => src.Value);

            CreateMap<Category, string>().ConvertUsing(src => src.ToString());
            CreateMap<Priority, string>().ConvertUsing(src => src.ToString());
            CreateMap<Status, string>().ConvertUsing(src => src.ToString());

            CreateMap<Domain.Comment.Comment, CommentResult>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.Value));

            CreateMap<Domain.Ticket.Ticket, TicketDetailsResult>();
            CreateMap<Domain.Ticket.Ticket, TicketResult>();
        }
    }

}
