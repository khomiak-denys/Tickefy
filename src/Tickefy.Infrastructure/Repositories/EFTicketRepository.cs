using Google.GenAI.Types;
using Microsoft.EntityFrameworkCore;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Common.Status;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.Tickets;
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

        public async Task<(int TotalCount, List<Ticket> Items)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Tickets.AsNoTracking();
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                  .Include(t => t.Requester)
                  .Include(t => t.AssignedAgent)
                  .Include(t => t.AssignedTeam)
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync(cancellationToken);
            return (totalCount, items);
        }

        public async Task<Ticket?> GetByIdAsync(TicketId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tickets
                .Include(t => t.Requester)
                .Include(t => t.AssignedAgent)
                .Include(t => t.AssignedTeam)
                .Include(t => t.Comments)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<(int TotalCount, List<Ticket> Items)> GetByUserIdAsync(UserId id, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Tickets
                .Where(t => (t.RequesterId == id || t.AssignedAgentId == id) && t.Status != Status.Canceled)
                .AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Include(t => t.Requester)
                .Include(t => t.AssignedAgent)
                .Include(t => t.AssignedTeam)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (totalCount, items);
        }

        public async Task<(int TotalCount, List<Ticket> Items)> GetCreatedByCategoryAsync(Category category, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Tickets
                .Where(t => t.Category == category && t.Status == Status.Created);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (totalCount, items);
        }

        public void Update(Ticket ticket)
        {
            _dbContext.Tickets.Update(ticket);
        }
    }
}
