using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.User.Common;

namespace Tickefy.Application.User.GetAll
{
    public class GetAllUsersQuery : IQuery<List<UserResult>>
    {
        public GetAllUsersQuery() { }
    }
}
