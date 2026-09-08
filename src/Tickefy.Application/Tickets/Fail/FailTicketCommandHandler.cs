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

namespace Tickefy.Application.Tickets.Fail;

public class FailTicketCommandHandler : ICommandHandler<FailTicketCommand, Result>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IActivityLogRepository _logRepository;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<FailTicketCommandHandler> _logger;

    public FailTicketCommandHandler(
        ITicketRepository ticketRepository,
        IActivityLogRepository logRepository,
        IUnitOfWork uow,
        ILogger<FailTicketCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _logRepository = logRepository;
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result> Handle(FailTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

        if (ticket == null)
        {
            _logger.LogWarning("Ticket {TicketId} not found when attempting to fail ticket", command.TicketId.Value);
            return Result.Failure(new NotFoundError(nameof(ticket) + " " + command.TicketId));
        }

        if (TicketAction.Fail.CanExecute(ticket, command.UserId, command.Roles))
        {
            ticket.Fail();
            var log = Domain.ActivityLogs.ActivityLog.Create(ticket.Id, command.UserId, EventType.StatusChanged, $"Ticket failed. Reason: {command.Reason}");
            _logRepository.Add(log);
            await _uow.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Ticket {TicketId} failed by user {UserId}. Reason: {Reason}", ticket.Id.Value, command.UserId.Value, command.Reason);
        }
        else
        {
            _logger.LogWarning("User {UserId} with roles {Roles} forbidden from failing ticket {TicketId}", command.UserId.Value, command.Roles, command.TicketId.Value);
            return Result.Failure(new ForbiddenError("Only admin can fail tickets"));
        }

        return Result.Success();
    }
}
