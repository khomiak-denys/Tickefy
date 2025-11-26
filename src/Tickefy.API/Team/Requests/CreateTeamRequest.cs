using Tickefy.Application.Team.Create;
using Tickefy.Domain.Common.Category;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Team.Requests
{
    public class CreateTeamRequest
    {
        public string Name { get; init; }
        public string? Description { get; init; }
        public Category Category { get; init; }

        public CreateTeamCommand ToCommand(UserId userid)
        {
            return new CreateTeamCommand(userid, Name, Description, Category);
        }
    }
}
