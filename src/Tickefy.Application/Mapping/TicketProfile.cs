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
            CreateMap<Domain.Comment.Comment, CommentResult>();
            CreateMap<Domain.Attachment.Attachment, AttachmentResult>();

            CreateMap<Domain.Ticket.Ticket, TicketDetailsResult>();
            CreateMap<Domain.Ticket.Ticket, TicketResult>();
        }
    }
}
