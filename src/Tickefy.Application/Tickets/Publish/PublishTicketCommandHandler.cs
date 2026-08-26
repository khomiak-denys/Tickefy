using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Attachments.Upload;
using Tickefy.Application.Tickets.Common.Helpers;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Attachments;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Priority;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tickets.Publish;

public class PublishTicketCommandHandler : ICommandHandler<PublishTicketCommand, Result<List<AttachmentUploadResult>>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiService _aiService;
    private readonly IAiResponseParser _responseParser;
    private readonly ILogger<PublishTicketCommandHandler> _logger;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IObjectStorageService _objectStorageService;

    public PublishTicketCommandHandler(
        ITicketRepository ticketRepository,
        IActivityLogRepository activityLogRepository,
        IUnitOfWork unitOfWork,
        IAiService aiService,
        IAiResponseParser responseParser,
        ILogger<PublishTicketCommandHandler> logger,
        IAttachmentRepository attachmentRepository,
        IObjectStorageService objectStorageService)
    {
        _ticketRepository = ticketRepository;
        _activityLogRepository = activityLogRepository;
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _responseParser = responseParser;
        _logger = logger;
        _attachmentRepository = attachmentRepository;
        _objectStorageService = objectStorageService;
    }

    public async Task<Result<List<AttachmentUploadResult>>> Handle(PublishTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            return Result<List<AttachmentUploadResult>>.Failure(new NotFoundError("Ticket not found."));
        }

        if (!TicketAction.Publish.CanExecute(ticket, command.UserId, command.Roles))
        {
            return Result<List<AttachmentUploadResult>>.Failure(new ForbiddenError("You are not allowed to publish this ticket. Only users with the required permissions (e.g., the ticket owner or users with appropriate roles) can publish a ticket that is in a publishable state."));
        }

        ticket.Publish(command.Title, command.Description, command.Deadline);

        try
        {
            var response = await _aiService.AnalyzeTicketAsync(ticket.Title, ticket.Description, ticket.Deadline, cancellationToken);

            var category = _responseParser.ParseCategory(response);
            var priority = _responseParser.ParsePriority(response);

            ticket.SetCategory(category);
            ticket.SetPriority(priority);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze ticket with AI. Setting default values.");
            ticket.SetCategory(Category.Other);
            ticket.SetPriority(Priority.Medium);
        }

        var log = ActivityLog.Create(
            command.TicketId,
            command.UserId,
            EventType.StatusChanged,
            "Ticket published.");

        _activityLogRepository.Add(log);

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

        return Result<List<AttachmentUploadResult>>.Success(fileUrls);
    }
}
