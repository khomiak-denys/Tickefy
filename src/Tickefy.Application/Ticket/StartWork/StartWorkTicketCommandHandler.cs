using MediatR;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Exceptions;
using Tickefy.Application.Ticket.Common.Helpers;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Action;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.StartWork;

public class StartWorkTicketCommandHandler : ICommandHandler<StartWorkTicketCommand, Unit>
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
    
    public async Task<Unit> Handle(StartWorkTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

        if (ticket == null) throw new NotFoundException(nameof(ticket), command.TicketId);
            
        if (TicketAction.StartWork.CanExecute(ticket, command.UserId, command.Roles))
        {
            ticket.StartWork();
            var log = Domain.ActivityLog.ActivityLog.Create(ticket.Id, command.UserId, EventType.StatusChanged,
                $"Agent started work on ticket.");
            _logRepository.Add(log);
            await _uow.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new ForbiddenException("Only assigned agent agent can start work tickets");
        }
        return Unit.Value;
    }
}