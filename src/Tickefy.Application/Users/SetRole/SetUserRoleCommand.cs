using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Users.SetRole
{
    public class SetUserRoleCommand : ICommand<Result>
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
