using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Users.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Users.GetById
{
    public class GetUserByIdQuery : IQuery<Result<UserDetailsResult>>
    {
        public UserId UserId { get; init; }

        public GetUserByIdQuery(UserId userId)
        {
            UserId = userId;
        }
    }
}
