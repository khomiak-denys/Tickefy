using Microsoft.Extensions.Logging;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Event;
using Tickefy.Domain.Common.Priority;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Tickets;

namespace Tickefy.Application.Tickets.Create
{
    internal class CreateTicketCommandHandler : ICommandHandler<CreateTicketCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITicketRepository _ticketRepository;
        private readonly IActivityLogRepository _logRepository;
        private readonly IAiService _aiService;
        private readonly IAiResponseParser _responseParser;
        private readonly ILogger<CreateTicketCommandHandler> _logger;

        public CreateTicketCommandHandler(
            IUnitOfWork uow,
            ITicketRepository ticketRepository,
            IActivityLogRepository logRepository,
            IAiService aiService,
            IAiResponseParser responseParser,
            ILogger<CreateTicketCommandHandler> logger)
        {
            _uow = uow;
            _ticketRepository = ticketRepository;
            _logRepository = logRepository;
            _aiService = aiService;
            _responseParser = responseParser;
            _logger = logger;
        }

        public async Task<Result> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = Ticket.Create(command.Title, command.Description, command.UserId, command.Deadline);

            try
            {
                var response = await _aiService.AnalyzeTicketAsync(ticket.Title, ticket.Description, ticket.Deadline, cancellationToken);

                var category = _responseParser.ParseCategory(response);
                var priority = _responseParser.ParsePriority(response);

                ticket.SetCategory(category);
                ticket.SetPriority(priority);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to analyze ticket with AI. Setting default values.");
                ticket.SetCategory(Category.Other);
                ticket.SetPriority(Priority.Medium);
            }

            _ticketRepository.Add(ticket);

            var log = ActivityLog.Create(ticket.Id, command.UserId, EventType.RequestCreated, "Created request");
            _logRepository.Add(log);

            await _uow.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
