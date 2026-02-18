using MediatR;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.CreateDraft;

public class CreateDraftTicketCommandHandler : ICommandHandler<CreateDraftTicketCommand, Unit>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDraftTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Unit> Handle(CreateDraftTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = Domain.Ticket.Ticket.CreateDraft(command.Title, command.Description, command.UserId, command.Deadline);
        
        _ticketRepository.Add(ticket);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}