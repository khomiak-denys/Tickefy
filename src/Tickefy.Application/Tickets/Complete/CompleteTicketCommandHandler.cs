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

namespace Tickefy.Application.Tickets.Complete
{
    public class CompleteTicketCommandHandler : ICommandHandler<CompleteTicketCommand, Result>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IActivityLogRepository _logRepository;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CompleteTicketCommandHandler> _logger;

        public CompleteTicketCommandHandler(
            ITicketRepository ticketRepository,
            IActivityLogRepository logRepository,
            IUnitOfWork uow,
            ILogger<CompleteTicketCommandHandler> logger)
        {
            _ticketRepository = ticketRepository;
            _logRepository = logRepository;
            _uow = uow;
            _logger = logger;
        }
        public async Task<Result> Handle(CompleteTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket {TicketId} not found when attempting to complete ticket", command.TicketId.Value);
                return Result.Failure(new NotFoundError(nameof(ticket) + " " + command.TicketId));
            }

            if (TicketAction.Complete.CanExecute(ticket, command.UserId, command.Roles))
            {
                ticket.Complete();
                var log = Domain.ActivityLogs.ActivityLog.Create(ticket.Id, command.UserId, EventType.StatusChanged,
                    "Ticket completed");
                _logRepository.Add(log);
                await _uow.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Ticket {TicketId} completed successfully by user {UserId}", ticket.Id.Value, command.UserId.Value);
            }
            else
            {
                _logger.LogWarning("User {UserId} with roles {Roles} forbidden from completing ticket {TicketId}", command.UserId.Value, command.Roles, command.TicketId.Value);
                return Result.Failure(new ForbiddenError("Only admin or assigned agent can complete tickets"));
            }

            return Result.Success();
        }
    }
}
