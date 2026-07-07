using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Users.Common;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Users.GetAll
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, Result<List<UserDetailsResult>>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersQueryHandler(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<List<UserDetailsResult>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            var result = users.Select(UserDetailsResult.FromEntity).ToList();

            return Result<List<UserDetailsResult>>.Success(result);
        }
    }
}
