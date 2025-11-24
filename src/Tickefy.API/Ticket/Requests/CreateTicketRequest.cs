using Tickefy.Domain.Primitives;
using Tickefy.Application.Ticket.Create;

namespace Tickefy.API.Ticket.Requests
{
    public class CreateTicketRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
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
