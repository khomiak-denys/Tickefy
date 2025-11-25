using Tickefy.Application.User.SetRole;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.User.Requests
{
    public class SetUserRoleRequest
    {
        public required string Role {  get; init; }

        public SetUserRoleCommand ToCommand(UserId userId)
        {
            return new SetUserRoleCommand(userId, Role);
        }
    }
}
