using Tickefy.API.Attachments.Requests;
using Tickefy.Application.Tickets.Create.Dto;
using Tickefy.Application.Tickets.Publish;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Ticket.Requests;

public class PublishTicketRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required DateTime Deadline { get; init; }
    public required List<UploadFileRequest> UploadFiles { get; init; }

    public PublishTicketCommand ToCommand(UserId userId, IEnumerable<string> roles, TicketId ticketId)
    {
        return new PublishTicketCommand
        {
            Title = Title,
            Description = Description,
            Deadline = Deadline,
            TicketId = ticketId,
            UserId = userId,
            Roles = roles,
            Files = UploadFiles.Select(file => new AttachmentFileItem(
                    file.ClientFileId, file.FileName, file.SizeBytes)).ToList()
        };
    }
}
