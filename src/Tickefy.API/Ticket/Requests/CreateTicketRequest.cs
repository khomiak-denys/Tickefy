using Tickefy.Domain.Primitives;
using Tickefy.Application.Tickets.Create;

namespace Tickefy.API.Ticket.Requests
{
    public class CreateTicketRequest
    {
        public required string Title { get; init; }
        public required string Description { get; init; }
        public required DateTime Deadline { get; init; }
        //public List<string> FileNames { get; set; }

        public CreateTicketCommand ToCommand(UserId userId)
        {
            return new CreateTicketCommand
            {
                UserId = userId,
                Title = Title,
                Description = Description,
                Deadline = Deadline
            };
        }
    }
}
