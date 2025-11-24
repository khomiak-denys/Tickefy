using AutoMapper;
using Tickefy.API.Ticket.Responses;
using Tickefy.Application.Ticket.Common;

namespace Tickefy.API.Mapping
{
    public class TicketMappingProfile : Profile
    {
        public TicketMappingProfile()
        {
            CreateMap<TicketResult, TicketResponse>();
            CreateMap<TicketDetailsResult, TicketDetailsResponse>();
            CreateMap<CommentResult, CommentResponse>();
            CreateMap<AttachmentResult, AttachmentResponse>();
        }
    }
}
