using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Attachments.Upload;
using Tickefy.Domain.Attachments;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tickets.CreateDraft;

public class CreateDraftTicketCommandHandler : ICommandHandler<CreateDraftTicketCommand, Result<List<AttachmentUploadResult>>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IObjectStorageService _objectStorageService;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly ILogger<CreateDraftTicketCommandHandler> _logger;

    public CreateDraftTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        IObjectStorageService objectStorageService,
        IAttachmentRepository attachmentRepository,
        ILogger<CreateDraftTicketCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _objectStorageService = objectStorageService;
        _attachmentRepository = attachmentRepository;
        _logger = logger;
    }

    public async Task<Result<List<AttachmentUploadResult>>> Handle(CreateDraftTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = Ticket.CreateDraft(command.Title, command.Description, command.UserId, command.Deadline);

        _ticketRepository.Add(ticket);

        var fileUrls = new List<AttachmentUploadResult>();
        foreach (var attachment in command.Files)
        {
            var parts = attachment.FileName.Split('.');
            var modifiedFileName = $"{parts[0]}_{Guid.NewGuid()}.{parts[1]}";

            var fileAttachment = Attachment.Create(modifiedFileName, attachment.SizeBytes, ticket.Id);

            await _attachmentRepository.AddAsync(fileAttachment);

            fileUrls.Add(new AttachmentUploadResult(
                attachment.ClientFileId,
                fileAttachment.Id.Value,
                parts[0],
                await _objectStorageService.GetUploadUrlAsync(fileAttachment.FilePath, attachment.SizeBytes)
                )
            );
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Draft ticket {TicketId} created successfully for user {UserId}", ticket.Id.Value, command.UserId.Value);

        return Result<List<AttachmentUploadResult>>.Success(fileUrls);
    }
}
