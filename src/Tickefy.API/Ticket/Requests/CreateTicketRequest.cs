using Tickefy.API.Attachments.Requests;
using Tickefy.Domain.Primitives;
using Tickefy.Application.Tickets.Create;
using Tickefy.Application.Tickets.Create.Dto;

namespace Tickefy.API.Ticket.Requests
{
    public class CreateTicketRequest
    {
        public required string Title { get; init; }
        public required string Description { get; init; }
        public required DateTime Deadline { get; init; }
        public required List<UploadFileRequest> UploadFiles { get; init; }

        public CreateTicketCommand ToCommand(UserId userId)
        {
            return new CreateTicketCommand
            {
                UserId = userId,
                Title = Title,
                Description = Description,
                Deadline = Deadline,
                Files = UploadFiles.Select(file => new AttachmentFileItem(
                    file.ClientFileId, file.FileName, file.SizeBytes)).ToList()
            };
        }
    }
}
