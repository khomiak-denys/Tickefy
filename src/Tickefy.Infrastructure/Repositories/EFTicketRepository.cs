using Microsoft.EntityFrameworkCore;
using Tickefy.Domain.Ticket;
using Tickefy.Infrastructure.Database;

namespace Tickefy.Infrastructure.Repositories
{
    public class EFTicketRepository : ITicketRepository
    {
        private readonly AppDbContext _dbContext;

        public EFTicketRepository(AppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public void Add(Ticket ticket)
        {
            _dbContext.Tickets.Add(ticket);
        }

        public void Delete(Ticket ticket)
        {
            _dbContext.Tickets.Remove(ticket);
        }

        public IEnumerable<Ticket> GetAll()
        {
            return _dbContext.Tickets.ToList();
        }

        public async Task<Ticket> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id.Value == id, cancellationToken);
        }

        public void Update(Ticket ticket)
        {
            _dbContext.Tickets.Update(ticket);
        }
    }
}
