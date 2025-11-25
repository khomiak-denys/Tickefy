using Microsoft.EntityFrameworkCore;
using Tickefy.Domain.Common.Status;
using Tickefy.Domain.Primitives;
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

        public async Task<IEnumerable<Ticket>> GetAll()
        {
            return await _dbContext.Tickets.AsNoTracking().ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(TicketId id, CancellationToken cancellationToken)
        {
            return await _dbContext.Tickets
                .Include(t => t.Comments)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<List<Ticket>> GetByUserId(UserId id)
        {
            return await _dbContext.Tickets.Where(t => t.RequesterId == id && t.Status != Status.Canceled).AsNoTracking().ToListAsync();
        }

        public void Update(Ticket ticket)
        {
            _dbContext.Tickets.Update(ticket);
        }
    }
}
