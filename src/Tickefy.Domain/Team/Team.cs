using Tickefy.Domain.Common.EntityBase;
using Tickefy.Domain.Primitives;

namespace Tickefy.Domain.Team
{
    public class Team : EntityBase<TeamId> 
    {
        public required string Name { get; set; }
        public string Description { get; set; }
        public List<UserId> Members { get; set; }
    }
}
