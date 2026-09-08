using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tickets.CreateDraft;

public class CreateDraftTicketCommandHandler : ICommandHandler<CreateDraftTicketCommand, Result>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDraftTicketCommandHandler> _logger;

    public CreateDraftTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateDraftTicketCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(CreateDraftTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = Ticket.CreateDraft(command.Title, command.Description, command.UserId, command.Deadline);

        _ticketRepository.Add(ticket);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Draft ticket {TicketId} created successfully for user {UserId}", ticket.Id.Value, command.UserId.Value);

        return Result.Success();
    }
}
