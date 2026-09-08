using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Attachments.Upload;
using Tickefy.Application.Tickets.Create.Dto;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Tickets.CreateDraft;

public class CreateDraftTicketCommand : ICommand<Result<List<AttachmentUploadResult>>>
{
    public required UserId UserId { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public DateTime Deadline { get; init; } = DateTime.Today;
    public required List<AttachmentFileItem> Files { get; init; }
}
