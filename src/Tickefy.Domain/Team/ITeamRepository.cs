using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Team
{
    /// <summary>
    /// Provides persistence operations for teams.
    /// </summary>
    public interface ITeamRepository
    {
        /// <summary>
        /// Adds a team to the repository.
        /// </summary>
        /// <param name="team">The team to add.</param>
        void Add(Team team);

        /// <summary>
        /// Deletes a team from the repository.
        /// </summary>
        /// <param name="team">The team to delete.</param>
        void Delete(Team team);

        /// <summary>
        /// Gets all teams.
        /// </summary>
        /// <returns>A list of all <see cref="Team"/> entities including manager information.</returns>
        Task<List<Team>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a team by its identifier.
        /// </summary>
        /// <param name="teamId">The identifier of the team.</param>
        /// <returns>The matching <see cref="Team"/> with members and manager included, or <see langword="null"/> if not found.</returns>
        Task<Team?> GetByIdAsync(TeamId teamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets teams that contain the specified member.
        /// </summary>
        /// <param name="memberId">The identifier of the team member.</param>
        /// <returns>A list of <see cref="Team"/> entities where the user is a manager or member.</returns>
        Task<List<Team>> GetByMemberIdAsync(UserId memberId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a team by its name.
        /// </summary>
        /// <param name="name">The team name.</param>
        /// <returns>The matching <see cref="Team"/> with members and manager included, or <see langword="null"/> if not found.</returns>
        Task<Team?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
