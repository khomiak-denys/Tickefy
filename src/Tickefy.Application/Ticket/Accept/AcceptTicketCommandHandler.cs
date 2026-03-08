using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Ticket.Common.Helpers;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.Accept;

public class AcceptTicketCommandHandler : ICommandHandler<AcceptTicketCommand, Result>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IActivityLogRepository _logRepository;
    private readonly IUnitOfWork _uow;

    public AcceptTicketCommandHandler(
        ITicketRepository ticketRepository,
        IActivityLogRepository logRepository,
        IUnitOfWork uow)
    {
        _ticketRepository = ticketRepository;
        _logRepository = logRepository;
        _uow = uow;
    }
    public async Task<Result> Handle(AcceptTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

        if (ticket == null) return Result.Failure(new NotFoundError(nameof(ticket)+ " " + command.TicketId));
            
        if (TicketAction.Accept.CanExecute(ticket, command.UserId, command.Roles))
        {
            ticket.Accept();
            var log = Domain.ActivityLog.ActivityLog.Create(ticket.Id, command.UserId, EventType.StatusChanged,
                $"Requester accepted ticket.");
            _logRepository.Add(log);
            await _uow.SaveChangesAsync(cancellationToken);
        }
        else
        {
            return Result.Failure(new ForbiddenError("Only requester can accept tickets"));
        }
        return Result.Success();
    }
}