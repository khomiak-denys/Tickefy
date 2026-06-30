using AutoMapper;
using Tickefy.Application.Ticket.Common;

namespace Tickefy.Application.Mapping
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<Domain.Comments.Comment, CommentResult>();
            CreateMap<Domain.Attachments.Attachment, AttachmentResult>();

            CreateMap<Domain.Tickets.Ticket, TicketResult>();
            CreateMap<Domain.Tickets.Ticket, TicketDetailsResult>()
                .ForMember(dest => dest.AvailableActions, opt => opt.Ignore());
        }
    }
}
