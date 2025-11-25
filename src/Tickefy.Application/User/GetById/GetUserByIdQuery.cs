using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.User.Common;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.User.GetById
{
    public class GetUserByIdQuery : IQuery<UserResult>
    { 
        public UserId UserId { get; init; }

        public GetUserByIdQuery(UserId userId) 
        {
            UserId = userId;
        }
    }
}
