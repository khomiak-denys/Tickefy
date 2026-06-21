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
        Task<List<Team>> GetAll();

        /// <summary>
        /// Gets a team by its identifier.
        /// </summary>
        /// <param name="teamId">The identifier of the team.</param>
        Task<Team?> GetByIdAsync(TeamId teamId);

        /// <summary>
        /// Gets teams that contain the specified member.
        /// </summary>
        /// <param name="memberId">The identifier of the team member.</param>
        Task<List<Team>> GetByMemberIdAsync(UserId memberId);

        /// <summary>
        /// Gets a team by its name.
        /// </summary>
        /// <param name="name">The team name.</param>
        Task<Team?> GetByNameAsync(string name);
    }
}
