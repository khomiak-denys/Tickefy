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

    public CreateDraftTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        IObjectStorageService objectStorageService,
        IAttachmentRepository attachmentRepository)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _objectStorageService = objectStorageService;
        _attachmentRepository = attachmentRepository;
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
                await _objectStorageService.GetUploadUrlAsync(fileAttachment.FilePath)
                )
            );
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<List<AttachmentUploadResult>>.Success(fileUrls);
    }
}
