using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Domain.Ticket;

namespace Tickefy.Application.Ticket.Create
{
    internal class CreateTicketCommandHandler : ICommandHandler<CreateTicketCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITicketRepository _ticketRepository;
        private readonly IAiService _aiService;
        private readonly IAiResponseParser _responseParser;

        public CreateTicketCommandHandler(
            IUnitOfWork uow, 
            ITicketRepository ticketRepository,
            IAiService aiService,
            IAiResponseParser responseParser)
        {
            _uow = uow;
            _ticketRepository = ticketRepository;
            _aiService = aiService;
            _responseParser = responseParser;
        }

        public async Task Handle(CreateTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = Domain.Ticket.Ticket.Create(command.Title, command.Description, command.UserId, command.Deadline);

            var response = await _aiService.AnalyzeTicketAsync(ticket.Title, ticket.Description, ticket.Deadline);

            var category = _responseParser.ParseCategory(response);
            var priority = _responseParser.ParsePriority(response);

            ticket.SetCategory(category);
            ticket.SetPriority(priority);

            _ticketRepository.Add(ticket);

            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
