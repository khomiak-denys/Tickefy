using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Tickets.Common.Helpers;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tickets.StartWork;

public class StartWorkTicketCommandHandler : ICommandHandler<StartWorkTicketCommand, Result>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IActivityLogRepository _logRepository;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<StartWorkTicketCommandHandler> _logger;

    public StartWorkTicketCommandHandler(
        ITicketRepository ticketRepository,
        IActivityLogRepository logRepository,
        IUnitOfWork uow,
        ILogger<StartWorkTicketCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _logRepository = logRepository;
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result> Handle(StartWorkTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

        if (ticket == null)
        {
            _logger.LogWarning("Ticket {TicketId} not found when attempting to start work", command.TicketId.Value);
            return Result.Failure(new NotFoundError(nameof(ticket) + " " + command.TicketId));
        }

        if (!TicketAction.StartWork.CanExecute(ticket, command.UserId, command.Roles))
        {
            _logger.LogWarning("User {UserId} with roles {Roles} forbidden from starting work on ticket {TicketId}", command.UserId.Value, command.Roles, command.TicketId.Value);
            return Result.Failure(new ForbiddenError("Only assigned agent agent can start work tickets"));
        }

        ticket.StartWork();
        var log = Domain.ActivityLogs.ActivityLog.Create(
            ticket.Id,
            command.UserId,
            EventType.StatusChanged,
            "Agent started work on ticket.");
        _logRepository.Add(log);
        await _uow.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Work started on ticket {TicketId} by agent {UserId}", ticket.Id.Value, command.UserId.Value);

        return Result.Success();
    }
}
