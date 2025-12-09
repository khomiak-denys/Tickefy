using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Team
{
    public interface ITeamRepository
    {
        void Add(Team team);
        void Delete(Team team);
        Task<List<Team>> GetAll();
        Task<Team?> GetByIdAsync(TeamId teamId);
        Task<List<Team>> GetByMemberIdAsync(UserId managerId);
        Task<Team?> GetByNameAsync(string name);
    }
}
