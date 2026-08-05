using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Users.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Users;
using Tickefy.Application.Common.Models;

namespace Tickefy.Application.Users.GetAll
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, Result<PaginationResult<UserDetailsResult>>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersQueryHandler(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<PaginationResult<UserDetailsResult>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            var totalCount = users.Count;

            var pagedUsers = users
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(UserDetailsResult.FromEntity)
                .ToList();

            var result = PaginationResult<UserDetailsResult>.Create(pagedUsers, query.PageNumber, query.PageSize, totalCount);

            return Result<PaginationResult<UserDetailsResult>>.Success(result);
        }
    }
}
