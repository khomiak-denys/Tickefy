using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Teams
{
    /// <summary>
    /// Defines contract requirements for persistence operations and lifecycle management of team aggregate roots.
    /// </summary>
    public interface ITeamRepository
    {
        /// <summary>
        /// Stages a new team entity for insertion into the database upon the subsequent transaction commit.
        /// </summary>
        /// <param name="team">
        /// The instantiated team aggregate root to add. Must possess a valid unique name and assigned manager; passing a detached or duplicate team entity may lead to unique constraint violations during save.
        /// </param>
        /// <remarks>
        /// This method performs an in-memory staging operation within the change tracker; changes are not persisted until the unit of work transaction is committed.
        /// </remarks>
        void Add(Team team);

        /// <summary>
        /// Marks an existing tracked team entity for deletion within the persistence context.
        /// </summary>
        /// <param name="team">
        /// The target team entity to remove. Ensure that any dependent entity associations or foreign key references are properly handled to prevent referential integrity exceptions during database commit.
        /// </param>
        /// <remarks>
        /// Deletion is deferred until the transaction commit. Attempting to delete a team that is currently untracked by the persistence context may require prior attachment or result in invalid operation exceptions.
        /// </remarks>
        void Delete(Team team);

        /// <summary>
        /// Asynchronously retrieves the complete collection of teams stored within the database, eagerly including their associated manager entities.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous query operation. The task result contains a list of all existing <see cref="Team"/> entities along with manager details.
        /// </returns>
        /// <remarks>
        /// Avoid calling this method without pagination filters in systems with large numbers of teams, as loading all teams and manager relationships simultaneously can induce memory contention and database latency.
        /// </remarks>
        Task<(int TotalCount, List<Team> Items)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously locates and retrieves a specific team entity by its unique domain identifier, eagerly loading its members and manager navigation properties.
        /// </summary>
        /// <param name="teamId">
        /// The strongly-typed identifier representing the primary key of the target team. Passing an uninitialized identifier will result in a null return value.
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous read operation. The task result contains the matching <see cref="Team"/> instance if found; otherwise, <see langword="null"/>.
        /// </returns>
        Task<Team?> GetByIdAsync(TeamId teamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves all teams in which the specified user participates either as the designated team manager or as an active team member.
        /// </summary>
        /// <param name="memberId">
        /// The strongly-typed domain identifier of the participating user. Passing a non-existent user identifier will result in an empty list rather than an exception.
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous query operation. The task result contains a list of <see cref="Team"/> entities associated with the specified user ID.
        /// </returns>
        Task<(int TotalCount, List<Team> Items)> GetByMemberIdAsync(UserId memberId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously searches for a team entity by its unique textual name, eagerly including its member and manager navigation relationships.
        /// </summary>
        /// <param name="name">
        /// The name string identifying the team. Depending on database collation settings, the lookup may be case-sensitive; callers should trim trailing whitespace before querying to avoid false negatives.
        /// </param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A task representing the asynchronous query operation. The task result contains the matching <see cref="Team"/> instance if found; otherwise, <see langword="null"/>.
        /// </returns>
        Task<Team?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
