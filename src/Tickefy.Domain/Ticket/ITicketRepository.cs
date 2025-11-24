using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Ticket
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAll();
        Task<Ticket?> GetByIdAsync(TicketId id, CancellationToken cancellationToken);
        Task<List<Ticket>> GetByUserId(UserId id);
        void Add(Ticket ticket);
        void Update(Ticket ticket);
        void Delete(Ticket ticket);
    }
}
