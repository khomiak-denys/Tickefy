using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.User.SetRole
{
    public class SetUserRoleCommand : ICommand<Unit>
    {
        public UserId UserId { get; init; }
        public string Role { get; init; }

        public SetUserRoleCommand(UserId userId, string role)
        {
            UserId = userId;
            Role = role;
        }
    }
}
