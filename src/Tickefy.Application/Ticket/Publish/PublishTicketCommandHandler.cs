using MediatR;
using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Common.Helpers;
using Tickefy.Application.Ticket.Create;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Priority;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.Publish;

public class PublishTicketCommandHandler : ICommandHandler<PublishTicketCommand, Unit>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiService _aiService;
    private readonly IAiResponseParser _responseParser;
    private readonly ILogger<PublishTicketCommandHandler> _logger;

    public PublishTicketCommandHandler(
        ITicketRepository ticketRepository,
        IActivityLogRepository activityLogRepository,
        IUnitOfWork unitOfWork,
        IAiService aiService,
        IAiResponseParser responseParser,
        ILogger<PublishTicketCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _activityLogRepository = activityLogRepository;
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _responseParser = responseParser;
        _logger = logger;
    }
    
    public async Task<Unit> Handle(PublishTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null) throw new NotFoundException("Ticket not found.");

        if (TicketAction.Publish.CanExecute(ticket, command.UserId, command.Roles))
        {
            ticket.Publish(command.Title, command.Description, command.Deadline);
            
            try
            {
                var response = await _aiService.AnalyzeTicketAsync(ticket.Title, ticket.Description, ticket.Deadline);

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

            var log = Domain.ActivityLog.ActivityLog.Create(
                command.TicketId, 
                command.UserId, 
                EventType.StatusChanged,
                "Ticket published.");
            
            _activityLogRepository.Add(log);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new ForbiddenException("Forbidden.");
        }
        return Unit.Value;
    }
}