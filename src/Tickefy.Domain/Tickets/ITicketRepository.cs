using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Tickets
{
    /// <summary>
    /// Defines contract requirements for persistence operations and querying capabilities related to ticket domain entities.
    /// </summary>
    public interface ITicketRepository
    {
        /// <summary>
        /// Asynchronously retrieves the complete collection of tickets stored within the database, including related navigation entities such as requester, assigned agent, and assigned team where configured by eager loading.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous read operation. The task result contains an enumerable sequence of all existing <see cref="Ticket"/> instances.
        /// </returns>
        /// <remarks>
        /// Caution: Calling this method on large production tables without pagination filters can cause excessive database IO and memory consumption. Prefer targeted queries when dealing with large volumes of tickets.
        /// </remarks>
        Task<(int TotalCount, List<Ticket> Items)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously locates a specific ticket entity by its unique domain identifier.
        /// </summary>
        /// <param name="id">
        /// The strongly-typed identifier representing the primary key of the ticket. Must be a valid, initialized identifier; passing an empty identifier will result in a null return value.
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous find operation. The task result contains the matching <see cref="Ticket"/> instance if found; otherwise, <see langword="null"/>.
        /// </returns>
        Task<Ticket?> GetByIdAsync(TicketId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously queries and retrieves all tickets created by a specific user.
        /// </summary>
        /// <param name="id">
        /// The strongly-typed domain identifier of the authoring user. Passing an invalid or uninitialized user identifier will return an empty list.
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous query operation. The task result contains a list of <see cref="Ticket"/> entities authored by the specified user.
        /// </returns>
        Task<(int TotalCount, List<Ticket> Items)> GetByUserIdAsync(UserId id, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stages a new ticket entity for insertion into the persistent storage context upon transaction commit.
        /// </summary>
        /// <param name="ticket">
        /// The instantiated ticket entity to be added. Must have a valid identifier and required domain fields configured. Passing a null reference or an entity that is already tracked in an incompatible state will cause tracking exceptions during persistence.
        /// </param>
        /// <remarks>
        /// This operation only registers the entity within the in-memory change tracker; changes are not persisted to the relational database until the unit of work saves changes.
        /// </remarks>
        void Add(Ticket ticket);

        /// <summary>
        /// Sets the state of an existing ticket entity to modified within the underlying change tracker, ensuring that updated properties are written during the next transaction commit.
        /// </summary>
        /// <param name="ticket">
        /// The modified ticket entity instance to update. Ensure that concurrent modification timestamps or concurrency tokens are valid to prevent silent data overwrite bugs.
        /// </param>
        /// <remarks>
        /// If the entity is already being tracked by the context, explicitly calling update may be unnecessary depending on change-tracking configuration, but calling it guarantees that all scalar and navigation property mutations are flagged for persistence.
        /// </remarks>
        void Update(Ticket ticket);

        /// <summary>
        /// Stages an existing ticket entity for removal from the database upon transaction commit.
        /// </summary>
        /// <param name="ticket">
        /// The target ticket entity to delete. The entity must be present in the database or currently tracked; attempting to remove an untracked or non-existent instance may throw concurrency or tracking exceptions.
        /// </param>
        /// <remarks>
        /// Deletion may cascade to dependent entities such as comments and attachments depending on database relational constraint definitions. Verify cascade behavior to prevent unintended data loss.
        /// </remarks>
        void Delete(Ticket ticket);

        /// <summary>
        /// Asynchronously retrieves all tickets assigned to a specific category that remain in the newly created state.
        /// </summary>
        /// <param name="category">
        /// The category classification used to filter tickets. Care must be taken to pass a valid category enum value or reference; undefined enum values may result in empty query results or runtime evaluation errors.
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous query operation. The task result contains a list of matching <see cref="Ticket"/> entities filtered by category and initial creation status.
        /// </returns>
        Task<(int TotalCount, List<Ticket> Items)> GetCreatedByCategoryAsync(Category category, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
