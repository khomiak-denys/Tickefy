using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Users.Common;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.Users.GetAll
{
    public class GetAllUsersQuery : IQuery<Result<List<UserDetailsResult>>>
    {
        public GetAllUsersQuery() { }
    }
}
