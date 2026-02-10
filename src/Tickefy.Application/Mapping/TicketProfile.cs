using AutoMapper;
using Tickefy.Application.Ticket.Common;

namespace Tickefy.Application.Mapping
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<Domain.Comment.Comment, CommentResult>();
            CreateMap<Domain.Attachment.Attachment, AttachmentResult>();

            CreateMap<Domain.Ticket.Ticket, TicketResult>();
            CreateMap<Domain.Ticket.Ticket, TicketDetailsResult>()
                .ForMember(dest => dest.AvailableActions, opt => opt.Ignore());
        }
    }
}
