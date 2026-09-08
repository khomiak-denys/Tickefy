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

namespace Tickefy.Application.Tickets.Accept;

public class AcceptTicketCommandHandler : ICommandHandler<AcceptTicketCommand, Result>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IActivityLogRepository _logRepository;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<AcceptTicketCommandHandler> _logger;

    public AcceptTicketCommandHandler(
        ITicketRepository ticketRepository,
        IActivityLogRepository logRepository,
        IUnitOfWork uow,
        ILogger<AcceptTicketCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _logRepository = logRepository;
        _uow = uow;
        _logger = logger;
    }
    public async Task<Result> Handle(AcceptTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

        if (ticket == null)
        {
            _logger.LogWarning("Ticket {TicketId} not found when attempting to accept ticket", command.TicketId.Value);
            return Result.Failure(new NotFoundError(nameof(ticket) + " " + command.TicketId));
        }

        if (TicketAction.Accept.CanExecute(ticket, command.UserId, command.Roles))
        {
            ticket.Accept();
            var log = Domain.ActivityLogs.ActivityLog.Create(ticket.Id, command.UserId, EventType.StatusChanged,
                $"Requester accepted ticket.");
            _logRepository.Add(log);
            await _uow.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Ticket {TicketId} accepted successfully by user {UserId}", ticket.Id.Value, command.UserId.Value);
        }
        else
        {
            _logger.LogWarning("User {UserId} with roles {Roles} forbidden from accepting ticket {TicketId}", command.UserId.Value, command.Roles, command.TicketId.Value);
            return Result.Failure(new ForbiddenError("Only requester can accept tickets"));
        }
        return Result.Success();
    }
}
