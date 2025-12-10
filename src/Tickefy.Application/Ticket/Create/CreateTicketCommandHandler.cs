using MediatR;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Priority;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.Create
{
    internal class CreateTicketCommandHandler : ICommandHandler<CreateTicketCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITicketRepository _ticketRepository;
        private readonly IActivityLogRepository _logRepository;
        private readonly IAiService _aiService;
        private readonly IAiResponseParser _responseParser;

        public CreateTicketCommandHandler(
            IUnitOfWork uow, 
            ITicketRepository ticketRepository,
            IActivityLogRepository logRepository,
            IAiService aiService,
            IAiResponseParser responseParser)
        {
            _uow = uow;
            _ticketRepository = ticketRepository;
            _logRepository = logRepository;
            _aiService = aiService;
            _responseParser = responseParser;
        }

        public async Task<Unit> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = Domain.Ticket.Ticket.Create(command.Title, command.Description, command.UserId, command.Deadline);

            try
            {
                var response = await _aiService.AnalyzeTicketAsync(ticket.Title, ticket.Description, ticket.Deadline);

                var category = _responseParser.ParseCategory(response);
                var priority = _responseParser.ParsePriority(response);

                ticket.SetCategory(category);
                ticket.SetPriority(priority);
            }
            catch (Exception)
            {
                ticket.SetCategory(Category.Other);
                ticket.SetPriority(Priority.Medium);
            }

            _ticketRepository.Add(ticket);

            var log = Domain.ActivityLog.ActivityLog.Create(ticket.Id, command.UserId, EventType.RequestCreated, "Created request");
            _logRepository.Add(log);
            
            await _uow.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}
