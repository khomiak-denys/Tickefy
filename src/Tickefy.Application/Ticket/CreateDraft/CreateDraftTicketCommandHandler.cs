using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Ticket.CreateDraft;

public class CreateDraftTicketCommandHandler : ICommandHandler<CreateDraftTicketCommand, Result>
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

    public async Task<Result> Handle(CreateDraftTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = Domain.Tickets.Ticket.CreateDraft(command.Title, command.Description, command.UserId, command.Deadline);

        _ticketRepository.Add(ticket);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
