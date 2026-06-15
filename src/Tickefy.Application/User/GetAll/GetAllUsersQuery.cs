using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.User.Common;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.User.GetAll
{
    public class GetAllUsersQuery : IQuery<Result<List<UserDetailsResult>>>
    {
        public GetAllUsersQuery() { }
    }
}
