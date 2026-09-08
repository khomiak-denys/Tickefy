using Microsoft.Extensions.Logging;
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
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public GetUserByIdQueryHandler(
            IUserRepository userRepository,
            ILogger<GetUserByIdQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task<Result<UserDetailsResult>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", query.UserId.Value);
                return Result<UserDetailsResult>.Failure(new NotFoundError(nameof(user) + " " + query.UserId.ToString()));
            }

            var result = UserDetailsResult.FromEntity(user);

            _logger.LogInformation("Retrieved user {UserId}", query.UserId.Value);

            return Result<UserDetailsResult>.Success(result);
        }
    }
}
