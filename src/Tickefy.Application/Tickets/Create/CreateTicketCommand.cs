using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Attachments.Upload;
using Tickefy.Application.Tickets.Create.Dto;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.Create
{
    public class CreateTicketCommand : ICommand<Result<List<AttachmentUploadResult>>>
    {
        public required UserId UserId { get; init; }
        public required string Title { get; init; }
        public required string Description { get; init; }
        public DateTime Deadline { get; init; }
        public required List<AttachmentFileItem> Files { get; init; }
    }
}
