using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.ActivityLog
{
    /// <summary>
    /// Provides persistence operations for activity log entries.
    /// </summary>
    public interface IActivityLogRepository
    {
        /// <summary>
        /// Adds an activity log entry to the repository.
        /// </summary>
        /// <param name="log">The activity log entry to add.</param>
        void Add(ActivityLog log);

        /// <summary>
        /// Gets a paged list of activity log entries.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of entries to include in the page.</param>
        /// <returns>A list of <see cref="ActivityLog"/> entries for the requested page, ordered by creation date descending.</returns>
        Task<List<ActivityLog>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets activity log entries for a ticket.
        /// </summary>
        /// <param name="ticketId">The identifier of the ticket.</param>
        /// <returns>A list of <see cref="ActivityLog"/> entries associated with the specified ticket.</returns>
        Task<List<ActivityLog>> GetByTicketIdAsync(TicketId ticketId, CancellationToken cancellationToken = default);
    }
}
