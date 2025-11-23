namespace Tickefy.Domain.Ticket
{
    public interface ITicketRepository
    {
        IEnumerable<Ticket> GetAll();
        Task<Ticket> GetByIdAsync(Guid id, CancellationToken cancellationToken); 
        void Add(Ticket ticket);
        void Update(Ticket ticket);
        void Delete(Ticket ticket);
    }
}
