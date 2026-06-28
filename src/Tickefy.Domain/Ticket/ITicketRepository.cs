using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Ticket
{
    /// <summary>
    /// Provides persistence operations for tickets.
    /// </summary>
    public interface ITicketRepository
    {
        /// <summary>
        /// Gets all tickets.
        /// </summary>
        /// <returns>A collection of all <see cref="Ticket"/> entities including related data.</returns>
        Task<IEnumerable<Ticket>> GetAll();

        /// <summary>
        /// Gets a ticket by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the ticket.</param>
        /// <param name="cancellationToken">The token used to cancel the operation.</param>
        /// <returns>The matching <see cref="Ticket"/>, or <see langword="null"/> if not found.</returns>
        Task<Ticket?> GetByIdAsync(TicketId id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets tickets created by the specified user.
        /// </summary>
        /// <param name="id">The identifier of the user.</param>
        /// <returns>A list of <see cref="Ticket"/> entities associated with the user.</returns>
        Task<List<Ticket>> GetByUserId(UserId id);

        /// <summary>
        /// Adds a ticket to the repository.
        /// </summary>
        /// <param name="ticket">The ticket to add.</param>
        void Add(Ticket ticket);

        /// <summary>
        /// Updates a ticket in the repository.
        /// </summary>
        /// <param name="ticket">The ticket to update.</param>
        void Update(Ticket ticket);

        /// <summary>
        /// Deletes a ticket from the repository.
        /// </summary>
        /// <param name="ticket">The ticket to delete.</param>
        void Delete(Ticket ticket);

        /// <summary>
        /// Gets created tickets assigned to the specified category.
        /// </summary>
        /// <param name="category">The category used to filter tickets.</param>
        /// <returns>A list of <see cref="Ticket"/> entities with the given category and a <c>Created</c> status.</returns>
        Task<List<Ticket>> GetCreatedByCategory(Category category);
    }
}
