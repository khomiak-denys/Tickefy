using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.ActivityLogs
{
    /// <summary>
    /// Defines contract requirements for persistence operations and structured retrieval of audit and activity log entries.
    /// </summary>
    public interface IActivityLogRepository
    {
        /// <summary>
        /// Stages a new audit log entry for insertion into the persistence database upon transaction commit.
        /// </summary>
        /// <param name="log">
        /// The instantiated activity log aggregate root to add. Must contain a valid timestamp, event classification, and associated entity identifiers; passing a null reference or duplicate entity may result in persistence failures.
        /// </param>
        /// <remarks>
        /// This method registers the log entity in the change tracker; actual insertion into the audit log storage table occurs when the unit of work saves changes.
        /// </remarks>
        void Add(ActivityLog log);

        /// <summary>
        /// Asynchronously retrieves a paginated slice of activity log records from the persistent audit store, ordered by creation date in descending sequence.
        /// </summary>
        /// <param name="page">
        /// The 1-based index of the page to retrieve. Passing a page number less than 1 may cause pagination offset calculation errors or throw argument exceptions depending on provider configuration.
        /// </param>
        /// <param name="pageSize">
        /// The maximum number of log records to return per page. Excessively large page sizes can lead to memory exhaustion and degraded database query performance; small values increase network round-trips.
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous paginated query. The task result contains the requested page of <see cref="ActivityLog"/> entries ordered from most recent to oldest.
        /// </returns>
        Task<(int TotalCount, List<ActivityLog> Items)> GetAllAsync(int Page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously queries and retrieves the chronological audit trail of activity log records associated with a specific ticket.
        /// </summary>
        /// <param name="ticketId">
        /// The strongly-typed identifier of the target ticket. Passing an uninitialized or empty ticket ID will result in an empty list return value without throwing an exception.
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous query operation. The task result contains all matching <see cref="ActivityLog"/> records linked to the specified ticket identifier.
        /// </returns>
        Task<(int TotalCount, List<ActivityLog> Items)> GetByTicketIdAsync(TicketId ticketId, int Page, int pageSize, CancellationToken cancellationToken = default);
    }
}
