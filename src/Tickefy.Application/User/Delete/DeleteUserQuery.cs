using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.User.Delete
{
    public class DeleteUserCommand : ICommand<Unit>
    {
        public UserId UserId { get; init; }

        public DeleteUserCommand(UserId userId)
        {
            UserId = userId;
        }
    }
}
