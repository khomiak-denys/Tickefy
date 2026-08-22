using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Users.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.Users.GetAll
{
    public class GetAllUsersQuery : IQuery<Result<PaginationResult<UserDetailsResult>>>
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;

        public GetAllUsersQuery() { }
    }
}
