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
            var pagedData = await _userRepository.GetAllAsync(query.Page, query.PageSize, cancellationToken);

            var pagedUsers = pagedData.Items
                .Select(UserDetailsResult.FromEntity)
                .ToList();

            var result = PaginationResult<UserDetailsResult>.Create(pagedUsers, query.Page, query.PageSize, pagedData.TotalCount);

            return Result<PaginationResult<UserDetailsResult>>.Success(result);
        }
    }
}
