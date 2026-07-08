using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Application.Users.Common;
using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;
using Tickefy.Domain.Users;

namespace Tickefy.Application.Users.GetById
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Result<UserDetailsResult>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<UserDetailsResult>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
            if (user == null) return Result<UserDetailsResult>.Failure(new NotFoundError(nameof(user) + " " + query.UserId.ToString()));

            var result = UserDetailsResult.FromEntity(user);
            return Result<UserDetailsResult>.Success(result);
        }
    }
}
