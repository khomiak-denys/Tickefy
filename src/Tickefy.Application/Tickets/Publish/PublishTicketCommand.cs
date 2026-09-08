using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Attachments.Upload;
using Tickefy.Application.Tickets.Create.Dto;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.Publish;

public class PublishTicketCommand : ICommand<Result<List<AttachmentUploadResult>>>
{
    public required UserId UserId { get; init; }
    public required IEnumerable<string> Roles { get; init; }
    public required TicketId TicketId { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required DateTime Deadline { get; init; }
    public required List<AttachmentFileItem> Files { get; init; }
}
