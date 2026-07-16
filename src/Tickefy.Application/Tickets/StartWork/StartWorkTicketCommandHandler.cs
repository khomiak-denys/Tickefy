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

    public StartWorkTicketCommandHandler(
        ITicketRepository ticketRepository,
        IActivityLogRepository logRepository,
        IUnitOfWork uow)
    {
        _ticketRepository = ticketRepository;
        _logRepository = logRepository;
        _uow = uow;
    }

    public async Task<Result> Handle(StartWorkTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

        if (ticket == null)
        {
            return Result.Failure(new NotFoundError(nameof(ticket) + " " + command.TicketId));
        }

        if (!TicketAction.StartWork.CanExecute(ticket, command.UserId, command.Roles))
        {
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

        return Result.Success();
    }
}
