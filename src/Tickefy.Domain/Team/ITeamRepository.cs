using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Team
{
    public interface ITeamRepository
    {
        void Add(Team team);
        void Delete(Team team);
        Task<Team?> GetByIdAsync(TeamId teamId);
        Task<Team?> GetByManagerIdAsync(UserId managerId);
    }
}
