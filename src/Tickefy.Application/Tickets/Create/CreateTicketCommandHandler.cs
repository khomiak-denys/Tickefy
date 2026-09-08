using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Attachments.Upload;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Attachments;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Priority;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tickets.Create
{
    internal sealed class CreateTicketCommandHandler : ICommandHandler<CreateTicketCommand, Result<List<AttachmentUploadResult>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITicketRepository _ticketRepository;
        private readonly IActivityLogRepository _logRepository;
        private readonly IAiService _aiService;
        private readonly IAiResponseParser _responseParser;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IObjectStorageService _objectStorageService;
        private readonly ILogger<CreateTicketCommandHandler> _logger;


        public CreateTicketCommandHandler(
            IUnitOfWork uow,
            ITicketRepository ticketRepository,
            IActivityLogRepository logRepository,
            IAiService aiService,
            IAiResponseParser responseParser,
            IAttachmentRepository attachmentRepository,
            IObjectStorageService objectStorageService,
            ILogger<CreateTicketCommandHandler> logger)
        {
            _uow = uow;
            _ticketRepository = ticketRepository;
            _logRepository = logRepository;
            _aiService = aiService;
            _responseParser = responseParser;
            _attachmentRepository = attachmentRepository;
            _objectStorageService = objectStorageService;
            _logger = logger;
        }

        public async Task<Result<List<AttachmentUploadResult>>> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = Ticket.Create(command.Title, command.Description, command.UserId, command.Deadline);

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
                _logger.LogWarning(ex, "Failed to analyze ticket with AI for ticket {Title}. Setting default values.", ticket.Title);
                ticket.SetCategory(Category.Other);
                ticket.SetPriority(Priority.Medium);
            }

            _ticketRepository.Add(ticket);

            var log = ActivityLog.Create(ticket.Id, command.UserId, EventType.RequestCreated, "Created request");
            _logRepository.Add(log);

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

            await _uow.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Ticket {TicketId} created successfully with title {Title} by user {UserId}", ticket.Id.Value, ticket.Title, command.UserId.Value);

            return Result<List<AttachmentUploadResult>>.Success(fileUrls);
        }
    }
}
