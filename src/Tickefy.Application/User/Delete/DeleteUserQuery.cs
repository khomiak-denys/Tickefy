using MediatR;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.User.Delete
{
    public class DeleteUserCommand : ICommand<Result>
    {
        public UserId UserId { get; init; }

        public DeleteUserCommand(UserId userId)
        {
            UserId = userId;
        }
    }
}
